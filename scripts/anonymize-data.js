#!/usr/bin/env node

require('dotenv').config();

const fs = require('fs');
const crypto = require('crypto');
const path = require('path');

// Configuration from .env file and command line arguments
const INPUT_FILE = process.argv[2] || './data-dump/spotify-data.sql';
const OUTPUT_FILE = process.argv[3] || INPUT_FILE.replace('.sql', '-anonymized.sql');
const PRESERVE_USER = process.env.PRESERVE_USER || 'gavinhow'; // Don't anonymize this user

// Cache for consistent ID mapping
const idMappings = new Map();

// Anonymization patterns for Spotify Statistics data
const ANONYMIZATION_RULES = [
  // Users table - preserve gavinhow user, anonymize others
  {
    pattern: /INSERT INTO "SpotifyTracker"\."Users" VALUES \('([^']+)', '([^']+)', '([^']+)', '([^']+)', '([^']+)', ([^,]+), '([^']+)', '([^']+)', ([^)]+)\);/g,
    replace: (match, id, created, modified, accessToken, refreshToken, expiresIn, tokenCreateDate, displayName, isDisabled) => {
      // Don't anonymize gavinhow user
      if (displayName && (displayName.toLowerCase().includes('gavinhow') || displayName === PRESERVE_USER)) {
        return match; // Return original unchanged
      }
      
      const anonId = getMappedId(id);
      const anonDisplayName = `User_${hashValue(displayName).substring(0, 8)}`;
      const anonAccessToken = generateFakeToken();
      const anonRefreshToken = generateFakeToken();
      
      return `INSERT INTO "SpotifyTracker"."Users" VALUES ('${anonId}', '${created}', '${modified}', '${anonAccessToken}', '${anonRefreshToken}', ${expiresIn}, '${tokenCreateDate}', '${anonDisplayName}', ${isDisabled});`;
    }
  },
  
  // Plays table - anonymize user references but preserve gavinhow
  {
    pattern: /INSERT INTO "SpotifyTracker"\."Plays" VALUES \('([^']+)', '([^']+)', '([^']+)', '([^']+)'\);/g,
    replace: (match, trackId, userId, timeOfPlay, id) => {
      const anonUserId = isGavinhowUserId(userId) ? userId : getMappedId(userId);
      const anonId = getMappedId(id);
      
      return `INSERT INTO "SpotifyTracker"."Plays" VALUES ('${trackId}', '${anonUserId}', '${timeOfPlay}', '${anonId}');`;
    }
  },
  
  // ImportLogs table - anonymize user references but preserve gavinhow
  {
    pattern: /INSERT INTO "SpotifyTracker"\."ImportLogs" VALUES \(([^,]+), '([^']+)', '([^']+)', ([^,]+), ([^,]+), '([^']*)', '([^']*)'\);/g,
    replace: (match, id, userId, importDateTime, tracksImported, isSuccessful, errorMessage, createdAt) => {
      const anonUserId = isGavinhowUserId(userId) ? userId : getMappedId(userId);
      
      return `INSERT INTO "SpotifyTracker"."ImportLogs" VALUES (${id}, '${anonUserId}', '${importDateTime}', ${tracksImported}, ${isSuccessful}, '${errorMessage}', '${createdAt}');`;
    }
  },
  
  // Friends table - handle friend relationships
  {
    pattern: /INSERT INTO "SpotifyTracker"\."Friends" VALUES \('([^']+)', '([^']+)', '([^']+)'\);/g,
    replace: (match, id, userId, friendUserId) => {
      const anonId = getMappedId(id);
      const anonUserId = isGavinhowUserId(userId) ? userId : getMappedId(userId);
      const anonFriendUserId = isGavinhowUserId(friendUserId) ? friendUserId : getMappedId(friendUserId);
      
      return `INSERT INTO "SpotifyTracker"."Friends" VALUES ('${anonId}', '${anonUserId}', '${anonFriendUserId}');`;
    }
  }
];

// Hash function for consistent anonymization
function hashValue(value) {
  return crypto.createHash('sha256').update(value + 'salt').digest('hex').substring(0, 36);
}

// Get or create a consistent mapped ID
function getMappedId(originalId) {
  if (!idMappings.has(originalId)) {
    // Generate a new UUID-like ID
    const hash = hashValue(originalId);
    const mappedId = [
      hash.substring(0, 8),
      hash.substring(8, 12),
      hash.substring(12, 16),
      hash.substring(16, 20),
      hash.substring(20, 32)
    ].join('-');
    idMappings.set(originalId, mappedId);
  }
  return idMappings.get(originalId);
}

// Generate fake tokens
function generateFakeToken() {
  return crypto.randomBytes(32).toString('hex');
}

// Check if a user ID belongs to gavinhow (we'll track this during processing)
let gavinhowUserId = null;
function isGavinhowUserId(userId) {
  return userId === gavinhowUserId;
}

function anonymizeData(content) {
  console.log('Identifying gavinhow user ID...');
  
  // First pass: find gavinhow's user ID
  const userMatch = content.match(/INSERT INTO "SpotifyTracker"\."Users" VALUES \('gavinhow'/i);
  
  if (userMatch) {
    gavinhowUserId = userMatch[1];
    console.log(`  Found gavinhow user ID.`);
  } else {
    console.log('  No gavinhow user found in data');
  }
  
  console.log('Applying anonymization rules...');
  
  let anonymizedContent = content;
  let totalReplacements = 0;
  
  ANONYMIZATION_RULES.forEach((rule, index) => {
    const beforeLength = anonymizedContent.length;
    anonymizedContent = anonymizedContent.replace(rule.pattern, rule.replace);
    const afterLength = anonymizedContent.length;
    
    // Count replacements (rough estimate)
    const matches = (content.match(rule.pattern) || []).length;
    if (matches > 0) {
      console.log(`  Rule ${index + 1}: Found ${matches} matches`);
      totalReplacements += matches;
    }
  });
  
  console.log(`✅ Applied ${totalReplacements} anonymization replacements`);
  console.log(`🔒 Preserved data for user: ${PRESERVE_USER}`);
  
  return anonymizedContent;
}

function main() {
  console.log('Starting data anonymization...');
  console.log(`Input file: ${INPUT_FILE}`);
  console.log(`Output file: ${OUTPUT_FILE}`);
  console.log(`Preserving user: ${PRESERVE_USER}`);
  
  // Check if input file exists
  if (!fs.existsSync(INPUT_FILE)) {
    console.error(`❌ Input file not found: ${INPUT_FILE}`);
    console.log('Usage: node anonymize-data.js <input-file> [output-file]');
    process.exit(1);
  }
  
  try {
    // Read the SQL dump file
    console.log('Reading input file...');
    const content = fs.readFileSync(INPUT_FILE, 'utf8');
    console.log(`📖 Read ${content.length} characters`);
    
    // Apply anonymization
    const anonymizedContent = anonymizeData(content);
    
    // Write anonymized data
    console.log('Writing anonymized data...');
    fs.writeFileSync(OUTPUT_FILE, anonymizedContent);
    
    // Display results
    const inputStats = fs.statSync(INPUT_FILE);
    const outputStats = fs.statSync(OUTPUT_FILE);
    
    console.log(`✅ Anonymization completed successfully!`);
    console.log(`📁 Original file: ${INPUT_FILE} (${(inputStats.size / 1024 / 1024).toFixed(2)} MB)`);
    console.log(`📁 Anonymized file: ${OUTPUT_FILE} (${(outputStats.size / 1024 / 1024).toFixed(2)} MB)`);
    console.log(`⚠️  Remember to review the anonymized data before using it!`);
    
  } catch (error) {
    console.error('❌ Error during anonymization:', error.message);
    process.exit(1);
  }
}

if (require.main === module) {
  main();
}

module.exports = { main, anonymizeData };