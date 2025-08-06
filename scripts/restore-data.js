#!/usr/bin/env node

require('dotenv').config();

const { execSync, spawn } = require('child_process');
const fs = require('fs');
const readline = require('readline');

// Configuration from .env file
const LOCAL_DB_CONFIG = {
  host: process.env.TARGET_DB_HOST || 'localhost',
  port: process.env.TARGET_DB_PORT || '5432',
  database: process.env.TARGET_DB_NAME || 'spotifystatistics',
  username: process.env.TARGET_DB_USER || 'postgres',
  password: process.env.TARGET_DB_PASSWORD || 'postgres'
};

const INPUT_FILE = process.argv[2] || './data-dump/spotify-data-anonymized.sql';

// Progress spinner function
function startSpinner(message) {
  const frames = ['⠋', '⠙', '⠹', '⠸', '⠼', '⠴', '⠦', '⠧', '⠇', '⠏'];
  let i = 0;
  
  process.stdout.write(message);
  
  const interval = setInterval(() => {
    process.stdout.write(`\r${message} ${frames[i]}`);
    i = (i + 1) % frames.length;
  }, 80);
  
  return {
    stop: (successMessage) => {
      clearInterval(interval);
      process.stdout.write(`\r${successMessage}\n`);
    },
    fail: (errorMessage) => {
      clearInterval(interval);
      process.stdout.write(`\r${errorMessage}\n`);
    }
  };
}

async function promptUser(question) {
  const rl = readline.createInterface({
    input: process.stdin,
    output: process.stdout
  });
  
  return new Promise((resolve) => {
    rl.question(question, (answer) => {
      rl.close();
      resolve(answer.toLowerCase().trim());
    });
  });
}

async function clearExistingData() {
  console.log('⚠️  This will DELETE all existing data in the local database!');
  const confirmation = await promptUser('Are you sure you want to continue? (yes/no): ');
  
  if (confirmation !== 'yes' && confirmation !== 'y') {
    console.log('❌ Operation cancelled by user');
    process.exit(0);
  }
  
  console.log('Clearing existing data...');
  
  // Set PGPASSWORD environment variable
  process.env.PGPASSWORD = LOCAL_DB_CONFIG.password;
  
  try {
    // Drop database first
    const dropDbCommand = [
      'psql',
      `--host=${LOCAL_DB_CONFIG.host}`,
      `--port=${LOCAL_DB_CONFIG.port}`,
      `--username=${LOCAL_DB_CONFIG.username}`,
      '--dbname=postgres',
      '--command',
      `"DROP DATABASE IF EXISTS ${LOCAL_DB_CONFIG.database};"`
    ].join(' ');
    
    execSync(dropDbCommand, { stdio: 'inherit' });
    console.log('✅ Database dropped');
    
    // Create database
    const createDbCommand = [
      'psql',
      `--host=${LOCAL_DB_CONFIG.host}`,
      `--port=${LOCAL_DB_CONFIG.port}`,
      `--username=${LOCAL_DB_CONFIG.username}`,
      '--dbname=postgres',
      '--command',
      `"CREATE DATABASE ${LOCAL_DB_CONFIG.database};"`
    ].join(' ');
    
    execSync(createDbCommand, { stdio: 'inherit' });
    console.log('✅ Database created');
    
  } catch (error) {
    console.error('❌ Error recreating database:', error.message);
    console.log('Please ensure you have permission to drop/create databases');
    console.log('You may need to connect as a superuser or database owner');
    throw error;
  }
}

async function restoreData() {
  // Get file size for better progress indication
  const stats = fs.statSync(INPUT_FILE);
  const fileSizeInMB = (stats.size / (1024 * 1024)).toFixed(1);
  
  // Set PGPASSWORD environment variable
  process.env.PGPASSWORD = LOCAL_DB_CONFIG.password;
  
  const spinner = startSpinner(`🔄 Restoring ${fileSizeInMB}MB of data...`);
  
  return new Promise((resolve, reject) => {
    // Create a temporary SQL file with performance optimizations
    const optimizedFile = INPUT_FILE.replace('.sql', '-optimized.sql');
    const content = fs.readFileSync(INPUT_FILE, 'utf8');
    
    // Add performance optimizations at the beginning (only safe runtime settings)
    const optimizedContent = `
-- Performance optimizations for bulk INSERT (universally safe)
SET synchronous_commit = off;
SET work_mem = '50MB';
SET maintenance_work_mem = '256MB';

-- Use explicit transaction for better performance
BEGIN;

${content}

-- Commit the transaction
COMMIT;

-- Reset settings to defaults
RESET synchronous_commit;
RESET work_mem;
RESET maintenance_work_mem;
`;
    
    fs.writeFileSync(optimizedFile, optimizedContent);
    
    const psql = spawn('psql', [
      `--host=${LOCAL_DB_CONFIG.host}`,
      `--port=${LOCAL_DB_CONFIG.port}`,
      `--username=${LOCAL_DB_CONFIG.username}`,
      `--dbname=${LOCAL_DB_CONFIG.database}`,
      '--set=ON_ERROR_STOP=on',  // Stop on first error
      `--file=${optimizedFile}`
    ], {
      env: { ...process.env, PGPASSWORD: LOCAL_DB_CONFIG.password },
      stdio: ['ignore', 'pipe', 'pipe']
    });
    
    let errorOutput = '';
    
    psql.stderr.on('data', (data) => {
      errorOutput += data.toString();
    });
    
    psql.on('close', (code) => {
      // Clean up temporary file
      try {
        fs.unlinkSync(optimizedFile);
      } catch (cleanupError) {
        // Ignore cleanup errors
      }
      
      if (code === 0) {
        spinner.stop('✅ Data restoration completed');
        resolve();
      } else {
        spinner.fail('❌ Data restoration failed');
        console.error('Error details:', errorOutput || `Process exited with code ${code}`);
        reject(new Error(`psql process exited with code ${code}`));
      }
    });
    
    psql.on('error', (error) => {
      spinner.fail('❌ Data restoration failed');
      console.error('Error details:', error.message);
      reject(error);
    });
  });
}

async function verifyRestore() {
  console.log('Verifying data restoration...');
  
  // Set PGPASSWORD environment variable
  process.env.PGPASSWORD = LOCAL_DB_CONFIG.password;
  
  const verifyCommand = [
    'psql',
    `--host=${LOCAL_DB_CONFIG.host}`,
    `--port=${LOCAL_DB_CONFIG.port}`,
    `--username=${LOCAL_DB_CONFIG.username}`,
    `--dbname=${LOCAL_DB_CONFIG.database}`,
    '--command',
    '"SELECT table_name, (SELECT COUNT(*) FROM information_schema.tables WHERE table_schema=\'SpotifyTracker\' AND table_name=t.table_name) as exists FROM (VALUES (\'Users\'), (\'Plays\'), (\'Tracks\'), (\'Artists\'), (\'Albums\'), (\'ImportLogs\'), (\'Friends\')) as t(table_name);"'
  ].join(' ');
  
  try {
    console.log('Database structure verification:');
    execSync(verifyCommand, { stdio: 'inherit' });
    
    // Count records in main tables
    const countCommand = [
      'psql',
      `--host=${LOCAL_DB_CONFIG.host}`,
      `--port=${LOCAL_DB_CONFIG.port}`,
      `--username=${LOCAL_DB_CONFIG.username}`,
      `--dbname=${LOCAL_DB_CONFIG.database}`,
      '--command',
      '"SELECT \'Users\' as table_name, COUNT(*) as count FROM \\"SpotifyTracker\\".\\"Users\\" UNION ALL SELECT \'Plays\', COUNT(*) FROM \\"SpotifyTracker\\".\\"Plays\\" UNION ALL SELECT \'Tracks\', COUNT(*) FROM \\"SpotifyTracker\\".\\"Tracks\\" UNION ALL SELECT \'ImportLogs\', COUNT(*) FROM \\"SpotifyTracker\\".\\"ImportLogs\\"; "'
    ].join(' ');
    
    console.log('\nRecord counts:');
    execSync(countCommand, { stdio: 'inherit' });
    
  } catch (error) {
    console.warn('⚠️  Verification failed, but restoration may have succeeded:', error.message);
  }
}

async function main() {
  console.log('Starting PostgreSQL data restoration...');
  console.log(`Input file: ${INPUT_FILE}`);
  console.log(`Target database: ${LOCAL_DB_CONFIG.database} on ${LOCAL_DB_CONFIG.host}:${LOCAL_DB_CONFIG.port}`);
  
  // Check if input file exists
  if (!fs.existsSync(INPUT_FILE)) {
    console.error(`❌ Input file not found: ${INPUT_FILE}`);
    console.log('Usage: node restore-data.js <input-file>');
    console.log('Make sure you have run the dump and anonymization scripts first');
    process.exit(1);
  }
  
  try {
    // Step 1: Clear existing data
    await clearExistingData();
    
    // Step 2: Restore the schema and data
    await restoreData();
    
    // Step 3: Verify the restoration
    await verifyRestore();
    
    console.log('\n🎉 Data restoration completed successfully!');
    console.log('Your local database now contains the anonymized production data');
    console.log('(with gavinhow user data preserved)');
    
  } catch (error) {
    console.error('\n❌ Data restoration failed:', error.message);
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