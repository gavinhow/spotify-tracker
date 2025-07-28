# Database Backup and Recovery Guide

This guide covers backup and recovery procedures for the PostgreSQL database in the Spotify Statistics application.

## Overview

The PostgreSQL database contains all your Spotify listening data, user information, and application state. Regular backups are essential to prevent data loss and enable disaster recovery.

## Backup Types

### 1. Manual Backups

#### Full Database Backup
```bash
# Create a full backup
docker-compose -f docker-compose.prod.yml exec postgres pg_dump -U spotifyuser -d spotify > backup_$(date +%Y%m%d_%H%M%S).sql

# Create a compressed backup
docker-compose -f docker-compose.prod.yml exec postgres pg_dump -U spotifyuser -d spotify | gzip > backup_$(date +%Y%m%d_%H%M%S).sql.gz
```

#### Schema-only Backup
```bash
# Backup only the database structure (no data)
docker-compose -f docker-compose.prod.yml exec postgres pg_dump -U spotifyuser -d spotify --schema-only > schema_backup_$(date +%Y%m%d_%H%M%S).sql
```

#### Data-only Backup
```bash
# Backup only the data (no structure)
docker-compose -f docker-compose.prod.yml exec postgres pg_dump -U spotifyuser -d spotify --data-only > data_backup_$(date +%Y%m%d_%H%M%S).sql
```

#### Table-specific Backup
```bash
# Backup specific table (e.g., Plays table)
docker-compose -f docker-compose.prod.yml exec postgres pg_dump -U spotifyuser -d spotify -t Plays > plays_backup_$(date +%Y%m%d_%H%M%S).sql
```

### 2. Automated Backups

#### Daily Backup Script
Create a script for automated daily backups:

```bash
#!/bin/bash
# save as backup-script.sh

BACKUP_DIR="/path/to/backups"
DATE=$(date +%Y%m%d_%H%M%S)
BACKUP_FILE="$BACKUP_DIR/spotify_backup_$DATE.sql.gz"

# Create backup directory if it doesn't exist
mkdir -p $BACKUP_DIR

# Create compressed backup
docker-compose -f docker-compose.prod.yml exec -T postgres pg_dump -U spotifyuser -d spotify | gzip > $BACKUP_FILE

# Check if backup was successful
if [ $? -eq 0 ]; then
    echo "Backup created successfully: $BACKUP_FILE"
    
    # Remove backups older than 30 days
    find $BACKUP_DIR -name "spotify_backup_*.sql.gz" -mtime +30 -delete
    echo "Old backups cleaned up"
else
    echo "Backup failed!"
    exit 1
fi
```

Make the script executable and add to cron:
```bash
chmod +x backup-script.sh

# Add to crontab for daily backup at 2 AM
crontab -e
# Add this line:
# 0 2 * * * /path/to/backup-script.sh
```

#### Using Docker Volume Backups
```bash
# Backup the entire PostgreSQL data directory
docker run --rm -v spotify_postgres_data:/data -v $(pwd)/backups:/backup alpine tar czf /backup/postgres_volume_backup_$(date +%Y%m%d_%H%M%S).tar.gz -C /data .
```

### 3. Continuous Backup with WAL-E or WAL-G

For enterprise setups, consider implementing continuous backup using WAL-E or WAL-G with cloud storage.

## Backup Storage

### Local Storage
- Store backups on a separate disk/partition
- Use RAID for redundancy
- Regular offsite copying

### Cloud Storage
```bash
# Example: Upload to AWS S3
aws s3 cp backup_$(date +%Y%m%d_%H%M%S).sql.gz s3://your-backup-bucket/spotify-backups/

# Example: Upload to Google Cloud Storage
gsutil cp backup_$(date +%Y%m%d_%H%M%S).sql.gz gs://your-backup-bucket/spotify-backups/
```

## Recovery Procedures

### 1. Full Database Restore

#### Stop the Application
```bash
docker-compose -f docker-compose.prod.yml stop web-api importer frontend nginx
```

#### Restore from Backup
```bash
# For uncompressed backup
docker-compose -f docker-compose.prod.yml exec -T postgres psql -U spotifyuser -d spotify < backup_20240127_143000.sql

# For compressed backup
gunzip -c backup_20240127_143000.sql.gz | docker-compose -f docker-compose.prod.yml exec -T postgres psql -U spotifyuser -d spotify
```

#### Restart the Application
```bash
docker-compose -f docker-compose.prod.yml start web-api importer frontend nginx
```

### 2. Point-in-Time Recovery

If you have continuous backup with WAL files:
```bash
# This requires WAL-E or WAL-G setup
# Restore to specific timestamp
wal-g backup-fetch /var/lib/postgresql/data LATEST
# Configure recovery.conf with target time
```

### 3. Partial Recovery

#### Restore Single Table
```bash
# Extract and restore specific table
pg_restore -U spotifyuser -d spotify -t Plays backup_file.dump
```

#### Restore Specific Data
```bash
# Restore data from specific date range
# First, create a filtered backup
docker-compose -f docker-compose.prod.yml exec postgres pg_dump -U spotifyuser -d spotify --where="\"CreatedAt\" >= '2024-01-01'" -t Plays > recent_plays.sql

# Then restore
docker-compose -f docker-compose.prod.yml exec -T postgres psql -U spotifyuser -d spotify < recent_plays.sql
```

## Backup Verification

### 1. Test Restore Process
```bash
# Create test database
docker-compose -f docker-compose.prod.yml exec postgres createdb -U spotifyuser spotify_test

# Restore backup to test database
docker-compose -f docker-compose.prod.yml exec -T postgres psql -U spotifyuser -d spotify_test < backup_file.sql

# Verify data integrity
docker-compose -f docker-compose.prod.yml exec postgres psql -U spotifyuser -d spotify_test -c "SELECT COUNT(*) FROM Plays;"

# Drop test database
docker-compose -f docker-compose.prod.yml exec postgres dropdb -U spotifyuser spotify_test
```

### 2. Backup Integrity Check
```bash
# Verify backup file is not corrupted
if gunzip -t backup_file.sql.gz; then
    echo "Backup file is valid"
else
    echo "Backup file is corrupted"
fi
```

## Monitoring and Alerting

### Backup Monitoring Script
```bash
#!/bin/bash
# backup-monitor.sh

BACKUP_DIR="/path/to/backups"
ALERT_EMAIL="admin@yourdomain.com"

# Check if backup exists from last 24 hours
LATEST_BACKUP=$(find $BACKUP_DIR -name "spotify_backup_*.sql.gz" -mtime -1 | head -n 1)

if [ -z "$LATEST_BACKUP" ]; then
    echo "No recent backup found!" | mail -s "Spotify DB Backup Alert" $ALERT_EMAIL
    exit 1
fi

# Check backup size (should be reasonable)
BACKUP_SIZE=$(stat -c%s "$LATEST_BACKUP")
MIN_SIZE=1048576  # 1MB minimum

if [ $BACKUP_SIZE -lt $MIN_SIZE ]; then
    echo "Backup file too small: $BACKUP_SIZE bytes" | mail -s "Spotify DB Backup Alert" $ALERT_EMAIL
    exit 1
fi

echo "Backup verification successful: $LATEST_BACKUP ($BACKUP_SIZE bytes)"
```

## Disaster Recovery Plan

### 1. Complete System Failure
1. **Set up new environment** with Docker Compose
2. **Restore database** from latest backup
3. **Verify data integrity**
4. **Update DNS/Load balancer** to point to new system
5. **Test application functionality**

### 2. Data Corruption
1. **Identify corruption scope** (single table vs. entire database)
2. **Stop application** to prevent further corruption
3. **Restore from clean backup** (before corruption occurred)
4. **Re-import recent data** if possible from Spotify API
5. **Restart application**

### 3. Partial Data Loss
1. **Assess what data is lost**
2. **Restore specific tables** from backup
3. **Use Spotify API** to refetch recent listening history
4. **Verify data consistency**

## Best Practices

1. **Regular Testing**: Test restore procedures monthly
2. **Multiple Backup Locations**: Store backups in multiple locations
3. **Retention Policy**: Keep daily backups for 30 days, weekly for 3 months, monthly for 1 year
4. **Monitoring**: Implement automated backup verification
5. **Documentation**: Keep recovery procedures updated
6. **Access Control**: Secure backup files with proper permissions
7. **Encryption**: Encrypt backups for sensitive data

## Backup Retention Schedule

```bash
# Example retention script
#!/bin/bash
BACKUP_DIR="/path/to/backups"

# Keep daily backups for 30 days
find $BACKUP_DIR -name "spotify_backup_*.sql.gz" -mtime +30 -delete

# Keep weekly backups (Sunday) for 12 weeks
find $BACKUP_DIR -name "spotify_weekly_backup_*.sql.gz" -mtime +84 -delete

# Keep monthly backups (1st of month) for 12 months
find $BACKUP_DIR -name "spotify_monthly_backup_*.sql.gz" -mtime +365 -delete
```

## Emergency Contacts

- **Database Administrator**: [Contact info]
- **System Administrator**: [Contact info]
- **Application Developer**: [Contact info]
- **Hosting Provider Support**: [Contact info]

Remember to update this information and test your backup/recovery procedures regularly!