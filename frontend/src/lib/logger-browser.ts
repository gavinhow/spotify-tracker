/**
 * Browser-safe logger for client-side usage
 *
 * This provides a simplified logging interface that works in the browser
 * without breaking SSR. For server-side logging, use the main logger.
 */

type LogLevel = 'debug' | 'info' | 'warn' | 'error' | 'fatal';

interface LogContext {
  [key: string]: unknown;
  err?: Error;
  error?: Error | { message: string; stack?: string };
}

/**
 * Converts camelCase to snake_case
 */
function toSnakeCase(str: string): string {
  return str.replace(/[A-Z]/g, (letter) => `_${letter.toLowerCase()}`);
}

/**
 * Formats log output for browser console
 */
function formatBrowserLog(
  level: LogLevel,
  context: LogContext,
  message: string
): Record<string, unknown> {
  const service =
    process.env.NEXT_PUBLIC_SERVICE_NAME ||
    'spotify-frontend';
  const environment = process.env.NEXT_PUBLIC_ENV || process.env.NODE_ENV || 'development';

  const log: Record<string, unknown> = {
    timestamp: new Date().toISOString(),
    level,
    message,
    service,
    environment,
  };

  // Handle error object
  const err = context.err || context.error;
  if (err) {
    if (err instanceof Error) {
      log.error = {
        type: err.name || 'Error',
        message: err.message,
        ...(err.stack && { stack_trace: err.stack }),
      };
    } else if (typeof err === 'object' && 'message' in err) {
      log.error = {
        type: 'Error',
        message: err.message,
        ...(err.stack && { stack_trace: err.stack }),
      };
    }
  }

  // Add other context properties with snake_case conversion
  for (const [key, value] of Object.entries(context)) {
    if (key !== 'err' && key !== 'error' && value !== undefined && value !== null) {
      const snakeCaseKey = toSnakeCase(key);
      log[snakeCaseKey] = value;
    }
  }

  return log;
}

/**
 * Logs to browser console with appropriate method
 */
function logToConsole(level: LogLevel, logObject: Record<string, unknown>): void {
  const { message, ...rest } = logObject;

  switch (level) {
    case 'debug':
      console.debug(message, rest);
      break;
    case 'info':
      console.info(message, rest);
      break;
    case 'warn':
      console.warn(message, rest);
      break;
    case 'error':
    case 'fatal':
      console.error(message, rest);
      break;
    default:
      console.log(message, rest);
  }
}

/**
 * Browser-safe logger interface matching pino API
 */
export const browserLogger = {
  debug(context: LogContext | string, message?: string): void {
    if (typeof context === 'string') {
      logToConsole('debug', formatBrowserLog('debug', {}, context));
    } else {
      logToConsole('debug', formatBrowserLog('debug', context, message || ''));
    }
  },

  info(context: LogContext | string, message?: string): void {
    if (typeof context === 'string') {
      logToConsole('info', formatBrowserLog('info', {}, context));
    } else {
      logToConsole('info', formatBrowserLog('info', context, message || ''));
    }
  },

  warn(context: LogContext | string, message?: string): void {
    if (typeof context === 'string') {
      logToConsole('warn', formatBrowserLog('warn', {}, context));
    } else {
      logToConsole('warn', formatBrowserLog('warn', context, message || ''));
    }
  },

  error(context: LogContext | string, message?: string): void {
    if (typeof context === 'string') {
      logToConsole('error', formatBrowserLog('error', {}, context));
    } else {
      logToConsole('error', formatBrowserLog('error', context, message || ''));
    }
  },

  fatal(context: LogContext | string, message?: string): void {
    if (typeof context === 'string') {
      logToConsole('fatal', formatBrowserLog('fatal', {}, context));
    } else {
      logToConsole('fatal', formatBrowserLog('fatal', context, message || ''));
    }
  },
};

/**
 * Optional: Send important client errors to server for logging
 *
 * SECURITY NOTE: Implementing a client error endpoint requires careful consideration:
 * - Rate limiting (prevent DoS/spam)
 * - Authentication (only allow logged-in users)
 * - Input validation and sanitization
 * - Size limits on error payloads
 * - Consider using an existing error tracking service (Sentry, Datadog, etc.) instead
 *
 * Example implementation with authentication:
 * ```typescript
 * export async function sendErrorToServer(
 *   error: Error,
 *   context: Record<string, unknown> = {}
 * ): Promise<void> {
 *   try {
 *     // Only send if user is authenticated
 *     const token = getCookie('token');
 *     if (!token) return;
 *
 *     await fetch('/api/log-error', {
 *       method: 'POST',
 *       headers: {
 *         'Content-Type': 'application/json',
 *         'Authorization': `Bearer ${token}`,
 *       },
 *       body: JSON.stringify({
 *         error: {
 *           type: error.name,
 *           message: error.message.substring(0, 500), // Limit size
 *           // Don't send full stack traces from client for security
 *         },
 *         context,
 *         timestamp: new Date().toISOString(),
 *       }),
 *     });
 *   } catch (err) {
 *     // Silently fail
 *     console.error('Failed to send error to server:', err);
 *   }
 * }
 * ```
 */
