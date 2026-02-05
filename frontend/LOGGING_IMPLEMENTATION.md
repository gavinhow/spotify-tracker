# Logging Implementation Summary

## What Was Implemented

A structured logging system using **pino** (already installed) that outputs logs in a Loki/Grafana-compatible JSON format.

## Files Created

1. **`src/lib/logger.ts`** - Main server-side logger
2. **`src/lib/logger-browser.ts`** - Client-side browser-safe logger
3. **`src/lib/logger-middleware.ts`** - Middleware for automatic API route logging
4. **`src/lib/LOGGING.md`** - Comprehensive documentation and examples
5. **`src/lib/__examples__/logger-examples.ts`** - Example usage

## Files Updated

1. **`src/lib/client.ts`** - Apollo Client now uses structured logging
2. **`src/app/api/authenticate/route.ts`** - Example with logging
3. **`src/app/api/logout/route.ts`** - Example with logging
4. **`.env.local`** - Added `SERVICE_NAME` and `LOG_LEVEL` variables

## Log Format

### Production Output (JSON)

```json
{
  "level": "info",
  "timestamp": "2026-02-05T18:32:33.157Z",
  "service": "frontend",
  "environment": "production",
  "user_id": "12345",
  "http": {
    "method": "POST",
    "path": "/api/playlists",
    "status_code": 200,
    "duration_ms": 45.2
  },
  "message": "Playlist created successfully"
}
```

### With Error

```json
{
  "level": "error",
  "timestamp": "2026-02-05T18:32:33.158Z",
  "service": "frontend",
  "environment": "production",
  "user_id": "12345",
  "playlist_id": "abc",
  "err": {
    "type": "ValidationError",
    "message": "Invalid playlist name",
    "stack_trace": "..."
  },
  "message": "Failed to create playlist"
}
```

### Development Output (Human-Readable)

Development uses `pino-pretty` for colorized, readable console output.

## Key Features

✅ **Automatic field transformations:**
- CamelCase → snake_case (e.g., `userId` → `user_id`)
- HTTP properties nested under `http` object
- Tracing properties at root level (`trace_id`, `span_id`, `request_id`)
- Errors nested under `err` object with proper formatting

✅ **Environment-specific output:**
- **Development**: Colorized, human-readable with pino-pretty
- **Production**: Compact JSON to stdout

✅ **Middleware for API routes:**
- Automatic HTTP request/response logging
- Duration tracking
- Custom context support

✅ **Browser-safe client logger:**
- Works with Next.js SSR
- Simplified interface for client-side logging

## Quick Start

### Server-Side Logging

```typescript
import { logger } from '@/lib/logger';

// Simple log
logger.info('Server started');

// With context
logger.info({ userId: '12345', action: 'login' }, 'User logged in');

// With HTTP context
logger.info(
  {
    method: 'POST',
    path: '/api/playlists',
    statusCode: 200,
    duration: 45.2,
    userId: '12345',
  },
  'API request completed'
);

// With error
try {
  await dangerousOperation();
} catch (error) {
  logger.error(
    {
      err: error,
      userId: '12345',
    },
    'Operation failed'
  );
}
```

### API Route with Middleware

```typescript
import { NextRequest, NextResponse } from 'next/server';
import { withLogging } from '@/lib/logger-middleware';

export async function POST(request: NextRequest) {
  return withLogging(request, async (req, log) => {
    // Add custom context
    log.userId = '12345';
    log.playlistId = 'abc';

    const result = await createPlaylist();
    return NextResponse.json(result);
  });
  // Automatically logs: POST /api/playlists responded 200 with duration
}
```

### Client-Side Logging

```typescript
import { browserLogger } from '@/lib/logger-browser';

// In React components
browserLogger.info('Component mounted');
browserLogger.error({ err: error, userId: '123' }, 'Failed to load data');
```

## Configuration

Environment variables in `.env.local`:

```bash
SERVICE_NAME=spotify-web
LOG_LEVEL=debug  # debug, info, warn, error, fatal
NODE_ENV=development  # or production
```

## Querying in Grafana/Loki

```logql
# All errors
{service="frontend"} | json | level="error"

# HTTP 500 errors
{service="frontend"} | json | http_status_code="500"

# Slow requests (> 1 second)
{service="frontend"} | json | http_duration_ms > 1000

# Specific user's requests
{service="frontend"} | json | user_id="12345"

# Specific endpoint
{service="frontend"} | json | http_path="/api/playlists"
```

## Implementation Approach

The logger uses **pino's built-in formatters and serializers** rather than fighting against its architecture:

1. **`base` binding** - Adds `service` and `environment` to every log
2. **`formatters.log`** - Transforms properties (camelCase → snake_case, nests HTTP properties)
3. **`serializers`** - Formats error objects with `type`, `message`, `stack_trace`
4. **`timestamp` function** - Adds ISO 8601 timestamps
5. **`messageKey`** - Uses `message` instead of default `msg`

This approach is simpler, more maintainable, and works with pino's design rather than against it.

## Next Steps

1. **Review the documentation**: See `src/lib/LOGGING.md` for comprehensive examples
2. **Update existing routes**: Add logging to your API routes using the middleware
3. **Configure log shipping**: Set up log forwarding to Loki (e.g., promtail, vector, fluent-bit)
4. **Add dashboards**: Create Grafana dashboards for monitoring HTTP requests, errors, and performance

## Note on Client Error Logging

The documentation originally included a `sendErrorToServer` function for sending client errors to the backend. This was intentionally removed due to security concerns (DoS potential, spam, abuse).

**Recommended alternatives:**
- Use a third-party service like Sentry, Datadog RUM, or LogRocket
- If you must implement your own, add authentication, rate limiting, and input validation

See `src/lib/logger-browser.ts` for a secure example implementation.

## Testing

Run the example file to see all log formats:

```bash
cd frontend
npx tsx src/lib/__examples__/logger-examples.ts
```

## Support

For more details, see:
- **Full documentation**: `src/lib/LOGGING.md`
- **Example implementations**: Updated API routes in `src/app/api/`
- **Pino documentation**: https://getpino.io/
