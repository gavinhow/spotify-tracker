# Docker Setup Guide

This guide covers how to deploy the Spotify Statistics application using Docker in production.

## Overview

The production Docker setup includes:
- **PostgreSQL Database** - Data storage
- **Web API** - ASP.NET Core GraphQL API
- **Frontend** - Next.js application
- **Importer** - Background service for data import
- **Nginx** - Reverse proxy and load balancer

## Quick Start

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd SpotifyStatistics
   ```

2. **Create environment file**
   ```bash
   cp .env.example .env
   ```

3. **Configure environment variables** (see Configuration section)

4. **Start the application**
   ```bash
   docker compose -f docker-compose.prod.yml up -d
   ```

5. **Run database migrations**
   ```bash
   docker compose -f docker-compose.prod.yml exec web-api dotnet ef database update
   ```

## Configuration

### Environment Variables

Create a `.env` file in the root directory with the following variables:

```bash
# Database
POSTGRES_USER=spotifyuser
POSTGRES_PASSWORD=your-secure-password

# Spotify API
SPOTIFY_CLIENT_ID=your-spotify-client-id
SPOTIFY_CLIENT_SECRET=your-spotify-client-secret
SPOTIFY_CALLBACK_URI=https://yourdomain.com/api/authenticate
SPOTIFY_SERVER_URI=https://yourdomain.com

# Application
JWT_SECRET=your-jwt-secret-key-at-least-32-characters
DEMO_API_KEY=your-demo-api-key

# Frontend
# Public API URL for client-side requests (from user's browser)
NEXT_PUBLIC_API_URL=https://yourdomain.com/graphql

# Note: Internal API URL for server-side requests is automatically configured
# in Docker Compose to use the internal network: http://web-api:8080
```

### Spotify API Setup

1. Go to [Spotify Developer Dashboard](https://developer.spotify.com/dashboard)
2. Create a new app
3. Add redirect URI: `https://yourdomain.com/api/authenticate`
4. Copy Client ID and Client Secret to your `.env` file

### SSL Configuration

For production with HTTPS:

1. **Obtain SSL certificates** (Let's Encrypt recommended)
   ```bash
   mkdir -p nginx/ssl
   # Copy your certificates to nginx/ssl/cert.pem and nginx/ssl/key.pem
   ```

2. **Update nginx configuration**
   - Edit `nginx/nginx.conf`
   - Uncomment the HTTPS server block
   - Update `server_name` to your domain

3. **Update environment variables**
   - Change `SPOTIFY_CALLBACK_URI` and `SPOTIFY_SERVER_URI` to use `https://`
   - Update `NEXT_PUBLIC_API_URL` to use `https://`

## API URL Configuration

The application uses a dual API URL configuration to handle Docker networking:

- **NEXT_PUBLIC_API_URL**: Used for client-side requests from the user's browser to the public-facing API
- **INTERNAL_API_URL**: Used for server-side requests within the Docker network (automatically configured)

This setup is necessary because:
1. **Client-side requests** (from browser) need to reach the public URL (through Nginx)
2. **Server-side requests** (from Next.js SSR) need to use the internal Docker network for better performance and reliability

The frontend automatically chooses the correct URL based on whether the request is happening server-side or client-side.

## Docker Compose Services

### PostgreSQL Database
- **Image**: `postgres:16-alpine`
- **Port**: 5432 (internal)
- **Volume**: `postgres_data` for persistence
- **Backups**: Available in `/backups` directory

### Web API
- **Build**: `Services/Dockerfile.web`
- **Port**: 8080 (internal), accessible via Nginx
- **Health Check**: `/health` endpoint
- **Dependencies**: PostgreSQL

### Frontend
- **Build**: `frontend/Dockerfile.prod`
- **Port**: 3000 (internal), accessible via Nginx
- **Dependencies**: Web API

### Importer
- **Build**: `Services/Dockerfile.importer`
- **Dependencies**: PostgreSQL, Web API
- **Purpose**: Background service for importing Spotify data

### Nginx
- **Image**: `nginx:alpine`
- **Ports**: 80, 443
- **Purpose**: Reverse proxy, SSL termination, rate limiting

## Common Commands

### Start Services
```bash
# Start all services
docker compose -f docker-compose.prod.yml up -d

# Start specific service
docker compose -f docker-compose.prod.yml up -d postgres

# View logs
docker compose -f docker-compose.prod.yml logs -f web-api
```

### Stop Services
```bash
# Stop all services
docker compose -f docker-compose.prod.yml down

# Stop and remove volumes (WARNING: This deletes data)
docker compose -f docker-compose.prod.yml down -v
```

### Database Operations
```bash
# Run migrations
docker compose -f docker-compose.prod.yml exec web-api dotnet ef database update

# Access database shell
docker compose -f docker-compose.prod.yml exec postgres psql -U spotifyuser -d spotify

# Create database backup
docker compose -f docker-compose.prod.yml exec postgres pg_dump -U spotifyuser spotify > backup.sql
```

### Monitoring
```bash
# Check service status
docker compose -f docker-compose.prod.yml ps

# View resource usage
docker stats

# Check logs
docker compose -f docker-compose.prod.yml logs -f
```

## Troubleshooting

### Service Won't Start
1. Check logs: `docker compose -f docker-compose.prod.yml logs <service-name>`
2. Verify environment variables are set
3. Ensure ports are not in use
4. Check Docker disk space

### Database Connection Issues
1. Verify PostgreSQL is running: `docker compose -f docker-compose.prod.yml ps postgres`
2. Check connection string in environment variables
3. Ensure database exists and migrations are applied

### SSL/HTTPS Issues
1. Verify certificates are in `nginx/ssl/` directory
2. Check nginx configuration syntax: `docker compose -f docker-compose.prod.yml exec nginx nginx -t`
3. Ensure firewall allows ports 80 and 443

### Performance Issues
1. Monitor resource usage: `docker stats`
2. Check nginx logs for rate limiting
3. Consider scaling services or increasing resources

## Security Considerations

1. **Use strong passwords** for database and JWT secret
2. **Keep Docker images updated** regularly
3. **Use HTTPS in production** with valid SSL certificates
4. **Limit database access** to application containers only
5. **Regular backups** of database and configuration
6. **Monitor logs** for suspicious activity
7. **Use non-root users** in containers (already configured)

## Updates and Maintenance

### Updating the Application
```bash
# Pull latest code
git pull origin main

# Rebuild and restart services
docker compose -f docker-compose.prod.yml build --no-cache
docker compose -f docker-compose.prod.yml up -d

# Run any new migrations
docker compose -f docker-compose.prod.yml exec web-api dotnet ef database update
```

### Health Monitoring
The setup includes health checks for critical services. Monitor these endpoints:
- Web API: `http://localhost:8080/health`
- Database: PostgreSQL health check built into Docker Compose

For production monitoring, consider implementing:
- External health check monitoring
- Log aggregation (ELK stack, Grafana)
- Metrics collection (Prometheus)
- Alerting systems