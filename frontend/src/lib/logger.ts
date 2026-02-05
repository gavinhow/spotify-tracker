import pino from 'pino';
import type { Logger as PinoLogger } from 'pino';

/**
 * Converts camelCase to snake_case
 */
function toSnakeCase(str: string): string {
  return str.replace(/[A-Z]/g, (letter) => `_${letter.toLowerCase()}`);
}

/**
 * Process bindings to add service and environment
 */
function formatBindings() {
  return {
    service: process.env.SERVICE_NAME || process.env.npm_package_name || 'spotify-web',
    environment: process.env.NODE_ENV || 'development',
  };
}

/**
 * Serializer for error objects
 */
function errorSerializer(err: Error) {
  return {
    type: err.name || 'Error',
    message: err.message,
    stack_trace: err.stack,
  };
}

/**
 * Transforms log properties to snake_case and nests HTTP properties
 */
function formatLogProperties(obj: Record<string, unknown>) {
  const formatted: Record<string, unknown> = {};
  const httpProps: Record<string, unknown> = {};

  // HTTP-related properties to nest
  const httpKeys = new Set(['method', 'path', 'url', 'query', 'headers']);

  for (const [key, value] of Object.entries(obj)) {
    if (value === null || value === undefined) continue;

    // Handle HTTP properties
    if (httpKeys.has(key)) {
      httpProps[key] = value;
      continue;
    }

    // Handle statusCode -> status_code
    if (key === 'statusCode') {
      httpProps.status_code = value;
      continue;
    }

    // Handle duration -> duration_ms
    if (key === 'duration') {
      httpProps.duration_ms = value;
      continue;
    }

    // Handle tracing properties
    if (key === 'traceId') {
      formatted.trace_id = value;
      continue;
    }
    if (key === 'spanId') {
      formatted.span_id = value;
      continue;
    }
    if (key === 'requestId') {
      formatted.request_id = value;
      continue;
    }

    // All other properties get snake_case conversion
    const snakeKey = toSnakeCase(key);
    formatted[snakeKey] = value;
  }

  // Add http object if we have http properties
  if (Object.keys(httpProps).length > 0) {
    formatted.http = httpProps;
  }

  return formatted;
}

/**
 * Creates a pino logger instance configured for the application
 */
function createLogger(): PinoLogger {
  const isDevelopment = process.env.NODE_ENV === 'development';

  const config = {
    level: process.env.LOG_LEVEL || (isDevelopment ? 'debug' : 'info'),
    base: formatBindings(),
    timestamp: () => `,"timestamp":"${new Date().toISOString()}"`,
    messageKey: 'message', // Use 'message' instead of 'msg'
    formatters: {
      level: (label: string) => {
        return { level: label };
      },
      log: formatLogProperties,
      bindings: formatBindings,
    },
    serializers: {
      err: errorSerializer,
      error: errorSerializer,
    },
  };

  // Development: use pino-pretty for human-readable output
  if (isDevelopment) {
    return pino({
      ...config,
      transport: {
        target: 'pino-pretty',
        options: {
          colorize: true,
          translateTime: 'SYS:standard',
          ignore: 'pid,hostname',
        },
      },
    });
  }

  // Production: compact JSON output
  return pino(config);
}

// Export singleton logger instance
export const logger = createLogger();

// Export type for convenience
export type Logger = PinoLogger;
