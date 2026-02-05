# Structured Logging Guide

This application uses [pino](https://getpino.io/) for structured logging with a standardized JSON format compatible with Loki/Grafana.

## Table of Contents

- [Quick Start](#quick-start)
- [Server-Side Logging](#server-side-logging)
- [Next.js Instrumentation Logging](#nextjs-instrumentation-logging)
- [Client-Side Logging](#client-side-logging)
- [API Route Middleware](#api-route-middleware)
- [Log Format](#log-format)
- [Field Conventions](#field-conventions)
- [Examples](#examples)

## Quick Start

### Server-Side (API Routes, Server Components)

```typescript
import { logger } from '@/lib/logger';

// Simple message
logger.info('Server started');

// With context
logger.info({ userId: '12345', action: 'login' }, 'User logged in');

// With error
logger.error({ err: error, userId: '12345' }, 'Failed to process request');
```

### Client-Side (Browser, Client Components)

```typescript
import { browserLogger } from '@/lib/logger-browser';

// Simple message
browserLogger.info('Component mounted');

// With context
browserLogger.error({ err: error, componentName: 'Playlist' }, 'Failed to load data');
```

## Server-Side Logging

### Basic Usage

```typescript
import { logger } from '@/lib/logger';

// Log levels
logger.debug({ debugInfo: 'detailed' }, 'Debug information');
logger.info({ userId: '123' }, 'Operation completed');
logger.warn({ threshold: 90 }, 'Memory usage high');
logger.error({ err: error }, 'Operation failed');
logger.fatal({ err: error }, 'Critical system failure');
```

## Next.js Instrumentation Logging

This project uses Next.js instrumentation (`src/instrumentation.ts`) to route framework-level server logs into the existing pino logger.

### What It Captures

- Next/server `console.debug`, `console.info`, `console.warn`, and `console.error` output on the server runtime
- Next request errors via the `onRequestError` instrumentation hook
- Source tags for easier filtering:
  - `source: "next_console"` for bridged console output
  - `source: "next_instrumentation"` for instrumentation lifecycle and request error logs

### What It Does Not Capture

- Browser console logs from client components
- Metrics exports (Prometheus/OpenTelemetry exporters)
- Any logs emitted outside the server runtime

### Dev vs Production Behavior

- **Development**: Logs still flow through the same logger, and `pino-pretty` keeps output readable
- **Production**: Logs are emitted as compact JSON to stdout with the same structured fields

### With HTTP Context

```typescript
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

// Output:
{
  "timestamp": "2024-02-04T10:30:00.123Z",
  "level": "info",
  "message": "API request completed",
  "service": "spotify-web",
  "environment": "production",
  "http": {
    "method": "POST",
    "path": "/api/playlists",
    "status_code": 200,
    "duration_ms": 45.2
  },
  "user_id": "12345"
}
```

### With Error Details

```typescript
try {
  await dangerousOperation();
} catch (error) {
  logger.error(
    {
      err: error,
      userId: '12345',
      playlistId: 'abc',
    },
    'Failed to create playlist'
  );
}

// Output:
{
  "timestamp": "2024-02-04T10:30:00.123Z",
  "level": "error",
  "message": "Failed to create playlist",
  "service": "spotify-web",
  "environment": "production",
  "user_id": "12345",
  "playlist_id": "abc",
  "error": {
    "type": "ValidationError",
    "message": "Invalid playlist name",
    "stack_trace": "ValidationError: Invalid playlist name\n  at ..."
  }
}
```

### With Tracing

```typescript
logger.info(
  {
    traceId: 'abc123',
    spanId: 'xyz789',
    userId: '12345',
  },
  'Processing request'
);

// Output:
{
  "timestamp": "2024-02-04T10:30:00.123Z",
  "level": "info",
  "message": "Processing request",
  "service": "spotify-web",
  "environment": "production",
  "trace_id": "abc123",
  "span_id": "xyz789",
  "user_id": "12345"
}
```

## Client-Side Logging

The browser logger provides a simplified interface for client-side logging:

```typescript
import { browserLogger } from '@/lib/logger-browser';

// In a React component
function PlaylistComponent() {
  useEffect(() => {
    browserLogger.info('Playlist component mounted');

    return () => {
      browserLogger.debug('Playlist component unmounted');
    };
  }, []);

  const handleError = (error: Error) => {
    browserLogger.error(
      {
        err: error,
        playlistId: '123',
        userId: currentUser.id,
      },
      'Failed to load playlist'
    );
  };
}
```

### Sending Client Errors to Server

**⚠️ Security Considerations:**

Creating an endpoint for client-side error logging requires careful security measures:
- **Rate limiting**: Prevent DoS attacks and spam
- **Authentication**: Only accept errors from authenticated users
- **Input validation**: Sanitize and validate all client input
- **Size limits**: Restrict payload sizes to prevent abuse
- **Consider using existing services**: Tools like Sentry, Datadog RUM, or LogRocket are purpose-built for this

**Recommended approach**: Use a third-party error tracking service rather than building your own endpoint. These services provide:
- Built-in security and rate limiting
- Source map support for minified code
- User session replay
- Better error grouping and analysis
- No risk of endpoint abuse

If you must implement your own endpoint, see `src/lib/logger-browser.ts` for a secure example implementation with authentication and size limits.

## API Route Middleware

### Using `withLogging` (Recommended)

Add custom context to your logs:

```typescript
import { NextRequest, NextResponse } from 'next/server';
import { withLogging } from '@/lib/logger-middleware';

export async function GET(request: NextRequest) {
  return withLogging(request, async (req, log) => {
    // Add custom context that will be included in logs
    log.userId = '12345';
    log.playlistId = 'abc';

    const data = await fetchData();

    return NextResponse.json(data);
  });
}

// Automatically logs:
// {
//   "timestamp": "...",
//   "level": "info",
//   "message": "GET /api/playlists responded 200",
//   "service": "spotify-web",
//   "http": {
//     "method": "GET",
//     "path": "/api/playlists",
//     "status_code": 200,
//     "duration_ms": 45.2
//   },
//   "trace_id": "...",
//   "span_id": "...",
//   "user_id": "12345",
//   "playlist_id": "abc"
// }
```

### Using `loggedRoute` (Simple)

When you don't need custom context:

```typescript
import { NextRequest, NextResponse } from 'next/server';
import { loggedRoute } from '@/lib/logger-middleware';

export async function GET(request: NextRequest) {
  return loggedRoute(request, async (req) => {
    const data = await fetchData();
    return NextResponse.json(data);
  });
}
```

### Manual Logging in Routes

```typescript
import { NextRequest, NextResponse } from 'next/server';
import { logger } from '@/lib/logger';

export async function POST(request: NextRequest) {
  const startTime = Date.now();
  const body = await request.json();

  try {
    const result = await processData(body);

    logger.info(
      {
        method: request.method,
        path: request.nextUrl.pathname,
        statusCode: 200,
        duration: Date.now() - startTime,
        userId: body.userId,
      },
      'Data processed successfully'
    );

    return NextResponse.json(result);
  } catch (error) {
    logger.error(
      {
        err: error instanceof Error ? error : new Error(String(error)),
        method: request.method,
        path: request.nextUrl.pathname,
        statusCode: 500,
        duration: Date.now() - startTime,
        userId: body.userId,
      },
      'Failed to process data'
    );

    return NextResponse.json({ error: 'Internal server error' }, { status: 500 });
  }
}
```

### GraphQL Operation Logging

```typescript
import { logGraphQLOperation } from '@/lib/logger-middleware';

// In your GraphQL client or Apollo Link
logGraphQLOperation({
  operationName: 'GetTopSongs',
  operationType: 'query',
  userId: '12345',
  duration: 145.2,
  success: true,
});
```

## Log Format

All logs follow this structure:

```typescript
{
  // Required fields (always present)
  timestamp: string;        // ISO 8601 UTC format
  level: string;           // debug, info, warn, error, fatal
  message: string;         // Log message
  service: string;         // Service name (from env or package.json)
  environment: string;     // development, staging, production

  // Optional: HTTP context (nested under 'http')
  http?: {
    method: string;
    path: string;
    status_code: number;
    duration_ms: number;
    url?: string;
    query?: Record<string, string>;
    headers?: Record<string, string>;
  };

  // Optional: Tracing (root level)
  trace_id?: string;
  span_id?: string;
  request_id?: string;

  // Optional: Error details (nested under 'error')
  error?: {
    type: string;
    message: string;
    stack_trace?: string;
  };

  // Optional: Custom properties (root level, snake_case)
  [key: string]: unknown;
}
```

## Field Conventions

### Naming Convention

- **All field names use `snake_case`**
- Custom properties are automatically converted from camelCase to snake_case
- Example: `userId` → `user_id`, `playlistId` → `playlist_id`

### Reserved Fields

These fields are managed by the logger and cannot be overridden by custom properties:

- `timestamp`
- `level`
- `message`
- `service`
- `environment`
- `http`
- `trace_id`
- `span_id`
- `request_id`
- `error`

### HTTP Properties

These properties are automatically nested under the `http` object:

- `method` → `http.method`
- `path` → `http.path`
- `statusCode` → `http.status_code`
- `duration` → `http.duration_ms`
- `url` → `http.url`
- `query` → `http.query`
- `headers` → `http.headers`

### Tracing Properties

These properties are converted to snake_case and placed at root level:

- `traceId` → `trace_id`
- `spanId` → `span_id`
- `requestId` → `request_id`

### Error Handling

Pass errors using the `err` property (pino convention):

```typescript
logger.error({ err: error }, 'Operation failed');
```

Errors are automatically formatted as:

```json
{
  "error": {
    "type": "TypeError",
    "message": "Cannot read property 'foo' of undefined",
    "stack_trace": "TypeError: Cannot read...\n  at ..."
  }
}
```

## Configuration

### Environment Variables

```bash
# Service name (defaults to package.json name)
SERVICE_NAME=spotify-web

# Environment (defaults to NODE_ENV)
NEXT_PUBLIC_ENV=production

# Log level (defaults to 'info' in production, 'debug' in development)
LOG_LEVEL=debug
```

### Development vs Production

**Development:**
- Uses `pino-pretty` for human-readable console output
- Default log level: `debug`
- Colorized output with timestamps

**Production:**
- Compact JSON output to stdout
- Default log level: `info`
- Optimized for log aggregation systems (Loki, CloudWatch, etc.)

## Examples

### Complete API Route Example

```typescript
import { NextRequest, NextResponse } from 'next/server';
import { withLogging } from '@/lib/logger-middleware';
import { logger } from '@/lib/logger';

export async function POST(request: NextRequest) {
  return withLogging(request, async (req, log) => {
    const body = await request.json();

    // Add context for automatic logging
    log.userId = body.userId;
    log.playlistId = body.playlistId;

    // Manual logging with additional context
    logger.debug({ requestBody: body }, 'Processing playlist creation request');

    try {
      const result = await createPlaylist(body);

      logger.info(
        {
          playlistId: result.id,
          trackCount: result.tracks.length,
        },
        'Playlist created successfully'
      );

      return NextResponse.json(result);
    } catch (error) {
      // Error is automatically logged by withLogging, but you can add more context
      logger.error(
        {
          err: error instanceof Error ? error : new Error(String(error)),
          attemptedOperation: 'createPlaylist',
          userId: body.userId,
        },
        'Playlist creation failed'
      );

      return NextResponse.json(
        { error: 'Failed to create playlist' },
        { status: 500 }
      );
    }
  });
}
```

### Querying Logs in Grafana/Loki

```logql
# All errors for the spotify-web service
{service="spotify-web"} | json | level="error"

# HTTP 500 errors
{service="spotify-web"} | json | http_status_code="500"

# Slow requests (> 1 second)
{service="spotify-web"} | json | http_duration_ms > 1000

# Specific user's requests
{service="spotify-web"} | json | user_id="12345"

# GraphQL errors
{service="spotify-web"} | json | level="error" | message=~"GraphQL.*"

# Requests to specific endpoint
{service="spotify-web"} | json | http_path="/api/playlists"
```

## Best Practices

1. **Always use structured logging**: Pass context as an object, not in the message string
   ```typescript
   // Good ✓
   logger.info({ userId: '123', action: 'login' }, 'User logged in');

   // Bad ✗
   logger.info(`User 123 logged in with action login`);
   ```

2. **Use appropriate log levels**:
   - `debug`: Detailed diagnostic information
   - `info`: Normal operational messages
   - `warn`: Warning messages for potentially harmful situations
   - `error`: Error events that might still allow the application to continue
   - `fatal`: Critical errors that may cause the application to abort

3. **Include tracing context** when available for distributed tracing

4. **Log at boundaries**: Log at entry/exit points of major operations

5. **Don't log sensitive data**: Avoid logging passwords, tokens, full credit card numbers, etc.

6. **Use middleware** for API routes to get consistent HTTP logging

7. **Keep messages concise**: The message should be human-readable; details go in the context object

## Troubleshooting

### Logs not appearing in development

Make sure `NODE_ENV=development` is set. Check if pino-pretty is installed:

```bash
pnpm install pino-pretty --save
```

### Custom properties not showing up

- Check that property names don't conflict with reserved fields
- Ensure values are not null, undefined, or empty
- Verify the property is passed in the context object, not the message

### Wrong field names in output

- The logger automatically converts camelCase to snake_case
- Check the field conventions section for how specific properties are handled

### Performance concerns

- Pino is one of the fastest Node.js loggers
- In production, logs are written asynchronously
- Avoid logging very large objects; log only necessary fields
