#!/usr/bin/env node

require('dotenv').config();

const { execSync } = require('child_process');
const crypto = require('crypto');

// Configuration from .env file
const LOCAL_DB_CONFIG = {
  host: process.env.TARGET_DB_HOST || 'localhost',
  port: process.env.TARGET_DB_PORT || '5432',
  database: process.env.TARGET_DB_NAME || 'spotifystatistics',
  username: process.env.TARGET_DB_USER || 'postgres',
  password: process.env.TARGET_DB_PASSWORD || 'postgres'
};

const PRESERVE_USER = process.env.PRESERVE_USER || 'gavinhow';

// Hash function for consistent anonymization
function hashValue(value) {
  return crypto.createHash('sha256').update(value + 'salt').digest('hex').substring(0, 36);
}

async function anonymizeDatabase() {
  console.log('🔄 Running anonymization queries on local database...');
  
  // Set PGPASSWORD environment variable
  process.env.PGPASSWORD = LOCAL_DB_CONFIG.password;
  
  try {
    // Step 1: Get count of users (excluding gavinhow)
    const countCommand = [
      'psql',
      `--host=${LOCAL_DB_CONFIG.host}`,
      `--port=${LOCAL_DB_CONFIG.port}`,
      `--username=${LOCAL_DB_CONFIG.username}`,
      `--dbname=${LOCAL_DB_CONFIG.database}`,
      '--tuples-only',
      '--command',
      `"SELECT COUNT(*) FROM \\"SpotifyTracker\\".\\"Users\\" WHERE \\"DisplayName\\" != '${PRESERVE_USER}';"`
    ].join(' ');
    
    const userCount = execSync(countCommand, { encoding: 'utf8' }).trim();
    console.log(`📊 Found ${userCount} users to anonymize (preserving ${PRESERVE_USER})`);
    
    // Step 2: Anonymize Users table (except gavinhow)
    console.log('🔄 Anonymizing Users table...');
    
    const anonymizeUsersCommand = [
      'psql',
      `--host=${LOCAL_DB_CONFIG.host}`,
      `--port=${LOCAL_DB_CONFIG.port}`,
      `--username=${LOCAL_DB_CONFIG.username}`,
      `--dbname=${LOCAL_DB_CONFIG.database}`,
      '--command',
      `"UPDATE \\"SpotifyTracker\\".\\"Users\\" SET 
         \\"DisplayName\\" = 'User_' || SUBSTRING(ENCODE(SHA256((COALESCE(\\"DisplayName\\", '') || 'salt')::bytea), 'hex'), 1, 8),
         \\"AccessToken\\" = ENCODE(SHA256((\\"Id\\" || 'access_salt')::bytea), 'hex'),
         \\"RefreshToken\\" = ENCODE(SHA256((\\"Id\\" || 'refresh_salt')::bytea), 'hex')
       WHERE \\"Id\\" != '${PRESERVE_USER}';"`
    ].join(' ');
    
    execSync(anonymizeUsersCommand, { stdio: 'inherit' });
    console.log('✅ Users table anonymized');
    
    // Step 3: Verify anonymization
    const verifyCommand = [
      'psql',
      `--host=${LOCAL_DB_CONFIG.host}`,
      `--port=${LOCAL_DB_CONFIG.port}`,
      `--username=${LOCAL_DB_CONFIG.username}`,
      `--dbname=${LOCAL_DB_CONFIG.database}`,
      '--command',
      `"SELECT COUNT(*) as anonymized_users FROM \\"SpotifyTracker\\".\\"Users\\" WHERE \\"DisplayName\\" LIKE 'User_%'; 
       SELECT COUNT(*) as preserved_users FROM \\"SpotifyTracker\\".\\"Users\\" WHERE \\"Id\\" = '${PRESERVE_USER}';"`
    ].join(' ');
    
    console.log('📊 Verification results:');
    execSync(verifyCommand, { stdio: 'inherit' });
    
  } catch (error) {
    console.error('❌ Error during database anonymization:', error.message);
    throw error;
  } finally {
    // Clean up environment variable
    delete process.env.PGPASSWORD;
  }
}

async function main() {
  console.log('Starting data anonymization directly on local database...');
  console.log(`Target database: ${LOCAL_DB_CONFIG.database} on ${LOCAL_DB_CONFIG.host}:${LOCAL_DB_CONFIG.port}`);
  console.log(`Preserving user: ${PRESERVE_USER}`);
  
  try {
    // Run anonymization directly on the database
    await anonymizeDatabase();
    
    console.log('\\n🎉 Database anonymization completed successfully!');
    console.log(`🔒 Preserved data for user: ${PRESERVE_USER}`);
    console.log('⚠️  All other user data has been anonymized');
    
  } catch (error) {
    console.error('\\n❌ Anonymization failed:', error.message);
    process.exit(1);
  }
}

if (require.main === module) {
  main();
}

module.exports = { main };