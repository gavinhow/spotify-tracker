import 'server-only';
import pino, { Logger } from 'pino';

const reservedFields = new Set([
  'timestamp',
  'level',
  'message',
  'service',
  'environment',
  'http',
  'trace_id',
  'span_id',
  'error'
]);

const httpPropertyNames = new Set([
  'Method',
  'Path',
  'StatusCode',
  'Protocol',
  'Scheme',
  'Host',
  'ElapsedMilliseconds',
  'Elapsed',
  'Duration'
]);

const serviceName =
  process.env.SERVICE_NAME ??
  process.env.NEXT_PUBLIC_SERVICE_NAME ??
  process.env.SERVICE ??
  'spotify-frontend';

const environment =
  process.env.VERCEL_ENV ??
  process.env.NODE_ENV ??
  'development';

export const logger = pino({
  level: process.env.LOG_LEVEL ?? 'info',
  messageKey: 'message',
  timestamp: () => `,"timestamp":"${new Date().toISOString()}"`,
  formatters: {
    level: (label) => ({ level: label }),
    bindings: () => ({ service: serviceName, environment }),
    log: (obj) => normalizeLogObject(obj)
  }
});

export type RequestHandler<T = Response> = (request: Request) => Promise<T> | T;

export async function withRequestLogging<T extends Response>(
  request: Request,
  handler: RequestHandler<T>,
  providedLogger: Logger = logger
): Promise<T> {
  const start = getNow();
  let response: T;
  try {
    response = await handler(request);
  } catch (error) {
    const elapsedMs = getNow() - start;
    const { traceId, spanId } = getTraceContext(request);
    const log = buildHttpLog(request, 500, elapsedMs, traceId, spanId);
    providedLogger.error({ ...log, err: error }, 'Request failed');
    throw error;
  }

  const elapsedMs = getNow() - start;
  const { traceId, spanId } = getTraceContext(request);
  const log = buildHttpLog(request, response.status, elapsedMs, traceId, spanId);
  providedLogger.info(log, 'Request %s %s responded %d', log.Method, log.Path, response.status);
  return response;
}

function normalizeLogObject(obj: Record<string, unknown>) {
  const http: Record<string, unknown> = {};
  const output: Record<string, unknown> = {};

  const traceId = takeSpecial(obj, ['TraceId', 'traceId']);
  const spanId = takeSpecial(obj, ['SpanId', 'spanId']);

  if (traceId) {
    output.trace_id = traceId;
  }

  if (spanId) {
    output.span_id = spanId;
  }

  for (const [key, value] of Object.entries(obj)) {
    if (isInternalProperty(key)) {
      continue;
    }

    if (httpPropertyNames.has(key)) {
      applyHttpProperty(http, key, value);
      continue;
    }

    if (key === 'TraceId' || key === 'traceId' || key === 'SpanId' || key === 'spanId') {
      continue;
    }

    if (key === 'err' || key === 'error') {
      const errorObject = toErrorObject(value);
      if (!isNullOrEmpty(errorObject)) {
        output.error = errorObject;
      }
      continue;
    }

    const snakeKey = toSnakeCase(key);
    if (reservedFields.has(snakeKey)) {
      continue;
    }

    const normalizedValue = normalizeValue(value);
    if (isNullOrEmpty(normalizedValue)) {
      continue;
    }

    output[snakeKey] = normalizedValue;
  }

  if (!isNullOrEmpty(http)) {
    output.http = http;
  }

  return output;
}

function buildHttpLog(request: Request, statusCode: number, elapsedMs: number, traceId?: string, spanId?: string) {
  const url = new URL(request.url);
  const protocol = request.headers.get('x-forwarded-proto') ?? url.protocol.replace(':', '');
  const host = request.headers.get('x-forwarded-host') ?? url.host;

  const log: Record<string, unknown> = {
    Method: request.method,
    Path: url.pathname,
    StatusCode: statusCode,
    Protocol: protocol,
    Scheme: protocol,
    Host: host,
    Duration: elapsedMs
  };

  if (traceId) {
    log.TraceId = traceId;
  }

  if (spanId) {
    log.SpanId = spanId;
  }

  return log;
}

function applyHttpProperty(http: Record<string, unknown>, key: string, value: unknown) {
  if (isNullOrEmpty(value)) {
    return;
  }

  switch (key) {
    case 'Method':
      http.method = value;
      break;
    case 'Path':
      http.path = value;
      break;
    case 'StatusCode':
      http.status_code = value;
      break;
    case 'Protocol':
      http.protocol = value;
      break;
    case 'Scheme':
      http.scheme = value;
      break;
    case 'Host':
      http.host = value;
      break;
    case 'ElapsedMilliseconds':
    case 'Elapsed':
    case 'Duration':
      http.duration_ms = toDurationMilliseconds(value);
      break;
  }
}

function toDurationMilliseconds(value: unknown) {
  if (typeof value === 'number') {
    return value;
  }

  if (typeof value === 'string') {
    const parsed = Number(value);
    return Number.isFinite(parsed) ? parsed : value;
  }

  return value;
}

function toErrorObject(value: unknown) {
  if (!value) {
    return null;
  }

  if (value instanceof Error) {
    return {
      type: value.name,
      message: value.message,
      stack_trace: value.stack
    };
  }

  if (typeof value === 'object' && value !== null) {
    const anyValue = value as Record<string, unknown>;
    if (typeof anyValue.type === 'string' || typeof anyValue.message === 'string') {
      return {
        type: typeof anyValue.type === 'string' ? anyValue.type : undefined,
        message: typeof anyValue.message === 'string' ? anyValue.message : undefined,
        stack_trace: typeof anyValue.stack_trace === 'string'
          ? anyValue.stack_trace
          : typeof anyValue.stack === 'string'
            ? anyValue.stack
            : undefined
      };
    }
  }

  return null;
}

function normalizeValue(value: unknown): unknown {
  if (Array.isArray(value)) {
    const normalized = value.map(normalizeValue).filter((item) => !isNullOrEmpty(item));
    return normalized.length > 0 ? normalized : null;
  }

  if (value instanceof Date) {
    return value.toISOString();
  }

  if (value && typeof value === 'object') {
    const output: Record<string, unknown> = {};
    for (const [key, child] of Object.entries(value)) {
      const normalized = normalizeValue(child);
      if (isNullOrEmpty(normalized)) {
        continue;
      }
      output[toSnakeCase(key)] = normalized;
    }
    return Object.keys(output).length > 0 ? output : null;
  }

  return value;
}

function isInternalProperty(name: string) {
  if (name.startsWith('@')) {
    return true;
  }

  return name === 'SourceContext' || name === 'MessageTemplate';
}

function isNullOrEmpty(value: unknown) {
  if (value === null || value === undefined) {
    return true;
  }

  if (typeof value === 'string') {
    return value.trim().length === 0;
  }

  if (Array.isArray(value)) {
    return value.length === 0;
  }

  if (typeof value === 'object') {
    return Object.keys(value as Record<string, unknown>).length === 0;
  }

  return false;
}

function toSnakeCase(value: string) {
  if (!value) {
    return value;
  }

  let result = '';
  for (let i = 0; i < value.length; i += 1) {
    const char = value[i];
    const prev = i > 0 ? value[i - 1] : '';
    const next = i + 1 < value.length ? value[i + 1] : '';

    if (char >= 'A' && char <= 'Z') {
      const hasPrev = i > 0;
      const shouldInsert =
        hasPrev &&
        (isLower(prev) || isDigit(prev) || (isUpper(prev) && next && isLower(next)));
      if (shouldInsert) {
        result += '_';
      }
      result += char.toLowerCase();
      continue;
    }

    if (char === '-' || char === ' ') {
      result += '_';
      continue;
    }

    result += char;
  }

  return result;
}

function isLower(char: string) {
  return char >= 'a' && char <= 'z';
}

function isUpper(char: string) {
  return char >= 'A' && char <= 'Z';
}

function isDigit(char: string) {
  return char >= '0' && char <= '9';
}

function getNow() {
  if (typeof performance !== 'undefined' && typeof performance.now === 'function') {
    return performance.now();
  }

  return Date.now();
}

function takeSpecial(obj: Record<string, unknown>, keys: string[]) {
  for (const key of keys) {
    const value = obj[key];
    if (typeof value === 'string' && value.trim().length > 0) {
      return value;
    }
  }
  return undefined;
}

function getTraceContext(request: Request) {
  const traceId = request.headers.get('x-trace-id') ?? undefined;
  const spanId = request.headers.get('x-span-id') ?? undefined;

  if (traceId || spanId) {
    return { traceId: traceId ?? undefined, spanId: spanId ?? undefined };
  }

  const traceparent = request.headers.get('traceparent');
  if (!traceparent) {
    return { traceId: undefined, spanId: undefined };
  }

  const parts = traceparent.split('-');
  if (parts.length < 4) {
    return { traceId: undefined, spanId: undefined };
  }

  return { traceId: parts[1], spanId: parts[2] };
}
