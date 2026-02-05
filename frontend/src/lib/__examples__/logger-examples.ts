/**
 * Example usage of the structured logger
 *
 * These examples demonstrate the various ways to use the logger
 * and the expected JSON output format.
 *
 * Run this file with: tsx src/lib/__examples__/logger-examples.ts
 */

import { logger } from '../logger';

console.log('=== Example 1: Simple Info Log ===\n');
logger.info('Application started');

console.log('\n=== Example 2: Log with Context ===\n');
logger.info({ userId: '12345', action: 'login' }, 'User logged in successfully');

console.log('\n=== Example 3: HTTP Request Log ===\n');
logger.info(
  {
    method: 'POST',
    path: '/api/playlists',
    statusCode: 200,
    duration: 45.2,
    userId: '12345',
    playlistId: 'abc',
  },
  'API request completed'
);

console.log('\n=== Example 4: Error Log ===\n');
const error = new Error('Database connection failed');
error.name = 'DatabaseError';
logger.error(
  {
    err: error,
    userId: '12345',
    operation: 'fetchPlaylists',
  },
  'Failed to fetch user playlists'
);

console.log('\n=== Example 5: Log with Tracing ===\n');
logger.info(
  {
    traceId: 'abc123def456',
    spanId: 'span789',
    requestId: 'req-001',
    method: 'GET',
    path: '/api/user/profile',
    statusCode: 200,
    duration: 23.5,
    userId: '12345',
  },
  'Profile request completed'
);

console.log('\n=== Example 6: Warning Log ===\n');
logger.warn(
  {
    threshold: 90,
    current: 95,
    metric: 'cpu_usage',
  },
  'CPU usage exceeds threshold'
);

console.log('\n=== Example 7: Debug Log (Development Only) ===\n');
logger.debug(
  {
    query: 'SELECT * FROM playlists',
    params: ['12345'],
    executionTime: 12.3,
  },
  'Database query executed'
);

console.log('\n=== Example 8: Complex Context with Mixed Properties ===\n');
logger.info(
  {
    userId: '12345',
    playlistName: 'My Favorites',
    trackCount: 42,
    isPublic: true,
    createdAt: new Date().toISOString(),
    method: 'POST',
    path: '/api/playlists',
    statusCode: 201,
    duration: 156.7,
    traceId: 'xyz789',
  },
  'Playlist created successfully'
);

console.log('\n=== Example 9: Empty Fields Are Excluded ===\n');
logger.info(
  {
    userId: '12345',
    playlistId: null, // Will be excluded
    tags: [], // Will be excluded
    description: '', // Will be excluded
    trackCount: 0, // Will be included (0 is not "empty")
  },
  'Testing empty field exclusion'
);

console.log('\n=== Example 10: Custom Properties with CamelCase ===\n');
logger.info(
  {
    userId: '12345',
    playlistId: 'abc',
    totalDuration: 3600,
    averageTrackLength: 180,
    mostPlayedArtist: 'Artist Name',
  },
  'Playlist statistics calculated'
);

console.log('\n✅ All examples completed!\n');
console.log('Note: In production (NODE_ENV=production), logs will be compact JSON.');
console.log('In development (NODE_ENV=development), logs are formatted with pino-pretty.\n');
