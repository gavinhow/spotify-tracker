import { NextRequest, NextResponse } from 'next/server';
import { logger } from './logger';

/**
 * Additional context to add to the request log
 */
export interface RequestLogContext {
  [key: string]: unknown;
  userId?: string;
  traceId?: string;
  spanId?: string;
  requestId?: string;
}

/**
 * Creates a middleware wrapper for Next.js API routes that automatically logs
 * HTTP request and response details.
 *
 * @example
 * ```typescript
 * export async function GET(request: NextRequest) {
 *   return withLogging(request, async (req, log) => {
 *     // Add custom context
 *     log.userId = '12345';
 *     log.playlistId = 'abc';
 *
 *     // Your route logic here
 *     const data = await fetchData();
 *
 *     return NextResponse.json(data);
 *   });
 * }
 * ```
 */
export async function withLogging<T extends NextResponse>(
  request: NextRequest,
  handler: (
    request: NextRequest,
    logContext: RequestLogContext
  ) => Promise<T>
): Promise<T> {
  const startTime = Date.now();
  const logContext: RequestLogContext = {};

  // Extract trace information if available (from headers or generate)
  const traceId =
    request.headers.get('x-trace-id') ||
    request.headers.get('x-request-id') ||
    crypto.randomUUID();
  const spanId = request.headers.get('x-span-id') || crypto.randomUUID().substring(0, 16);

  logContext.traceId = traceId;
  logContext.spanId = spanId;
  logContext.requestId = traceId;

  try {
    // Execute the handler
    const response = await handler(request, logContext);

    // Calculate duration
    const duration = Date.now() - startTime;

    // Log successful request
    logger.info(
      {
        method: request.method,
        path: request.nextUrl.pathname,
        statusCode: response.status,
        duration,
        traceId: logContext.traceId,
        spanId: logContext.spanId,
        requestId: logContext.requestId,
        ...logContext,
      },
      `${request.method} ${request.nextUrl.pathname} responded ${response.status}`
    );

    // Add trace headers to response
    response.headers.set('x-trace-id', traceId);
    response.headers.set('x-span-id', spanId);

    return response;
  } catch (error) {
    // Calculate duration even for errors
    const duration = Date.now() - startTime;

    // Log error
    logger.error(
      {
        err: error instanceof Error ? error : new Error(String(error)),
        method: request.method,
        path: request.nextUrl.pathname,
        statusCode: 500,
        duration,
        traceId: logContext.traceId,
        spanId: logContext.spanId,
        requestId: logContext.requestId,
        ...logContext,
      },
      `${request.method} ${request.nextUrl.pathname} failed`
    );

    // Re-throw to let Next.js handle the error
    throw error;
  }
}

/**
 * Simpler middleware wrapper that automatically logs but doesn't provide log context.
 * Use this when you don't need to add custom properties to logs.
 *
 * @example
 * ```typescript
 * export async function GET(request: NextRequest) {
 *   return loggedRoute(request, async (req) => {
 *     const data = await fetchData();
 *     return NextResponse.json(data);
 *   });
 * }
 * ```
 */
export async function loggedRoute<T extends NextResponse>(
  request: NextRequest,
  handler: (request: NextRequest) => Promise<T>
): Promise<T> {
  return withLogging(request, async (req, _logContext) => handler(req));
}

/**
 * Middleware that can be used in Next.js middleware.ts for global request logging
 *
 * @example
 * ```typescript
 * // In middleware.ts
 * import { logRequest } from '@/lib/logger-middleware';
 *
 * export function middleware(request: NextRequest) {
 *   logRequest(request);
 *   return NextResponse.next();
 * }
 * ```
 */
export function logRequest(request: NextRequest, context?: RequestLogContext): void {
  const traceId =
    request.headers.get('x-trace-id') ||
    request.headers.get('x-request-id') ||
    crypto.randomUUID();

  logger.info(
    {
      method: request.method,
      path: request.nextUrl.pathname,
      url: request.url,
      traceId,
      ...context,
    },
    `${request.method} ${request.nextUrl.pathname} received`
  );
}

/**
 * Logs GraphQL operations with query/mutation details
 *
 * @example
 * ```typescript
 * logGraphQLOperation({
 *   operationName: 'GetTopSongs',
 *   operationType: 'query',
 *   userId: '12345',
 *   duration: 145.2,
 *   success: true,
 * });
 * ```
 */
export function logGraphQLOperation(context: {
  operationName?: string;
  operationType?: 'query' | 'mutation' | 'subscription';
  userId?: string;
  duration?: number;
  success: boolean;
  error?: Error;
  variables?: Record<string, unknown>;
}): void {
  const { operationName, operationType, success, error, ...rest } = context;

  const message = `GraphQL ${operationType || 'operation'} ${operationName || 'unknown'} ${
    success ? 'completed' : 'failed'
  }`;

  if (success) {
    logger.info(rest, message);
  } else {
    logger.error(
      {
        err: error,
        ...rest,
      },
      message
    );
  }
}
