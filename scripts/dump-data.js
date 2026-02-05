#!/usr/bin/env node

require('dotenv').config();

const { execSync } = require('child_process');
const fs = require('fs');
const path = require('path');

// Configuration from .env file
const DB_CONFIG = {
  host: process.env.SOURCE_DB_HOST || 'localhost',
  port: process.env.SOURCE_DB_PORT || '5432',
  database: process.env.SOURCE_DB_NAME || 'spotifystatistics',
  username: process.env.SOURCE_DB_USER || 'postgres',
  password: process.env.SOURCE_DB_PASSWORD || 'postgres'
};

const OUTPUT_DIR = process.env.OUTPUT_DIR || './data-dump';
const TIMESTAMP = new Date().toISOString().replace(/[:.]/g, '-');
const OUTPUT_FILE = path.join(OUTPUT_DIR, `spotify-data-${TIMESTAMP}.pgdump`);

function main() {
  console.log('Starting PostgreSQL data dump...');
  
  // Create output directory if it doesn't exist
  if (!fs.existsSync(OUTPUT_DIR)) {
    fs.mkdirSync(OUTPUT_DIR, { recursive: true });
    console.log(`Created output directory: ${OUTPUT_DIR}`);
  }

  // Set PGPASSWORD environment variable
  process.env.PGPASSWORD = DB_CONFIG.password;

  // Build pg_dump command for custom format (binary, much faster)
  const dumpCommand = [
    'pg_dump',
    `--host=${DB_CONFIG.host}`,
    `--port=${DB_CONFIG.port}`,
    `--username=${DB_CONFIG.username}`,
    `--dbname=${DB_CONFIG.database}`,
    '--format=custom',      // Binary format for speed
    '--compress=6',         // Good compression level
    '--no-owner',          // Don't output ownership commands
    '--no-privileges',     // Don't output privilege commands
    `--file=${OUTPUT_FILE}`
  ].join(' ');

  try {
    console.log(`Dumping data from ${DB_CONFIG.database}...`);
    execSync(dumpCommand, { stdio: 'inherit' });
    
    console.log(`✅ Data dump completed successfully!`);
    console.log(`📁 Output file: ${OUTPUT_FILE}`);
    
    // Display file size
    const stats = fs.statSync(OUTPUT_FILE);
    const fileSizeInMB = (stats.size / (1024 * 1024)).toFixed(2);
    console.log(`📊 File size: ${fileSizeInMB} MB`);
    
  } catch (error) {
    console.error('❌ Error during data dump:', error.message);
    process.exit(1);
  } finally {
    // Clean up environment variable
    delete process.env.PGPASSWORD;
  }
}

if (require.main === module) {
  main();
}

module.exports = { main };