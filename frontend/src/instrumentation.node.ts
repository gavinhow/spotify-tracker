import type { Instrumentation } from 'next';
import { installConsoleBridge } from '@/lib/console-bridge';
import { logger } from '@/lib/logger';

function normalizeError(error: unknown): Error {
  if (error instanceof Error) {
    return error;
  }

  return new Error(typeof error === 'string' ? error : 'Unknown Next.js request error');
}

function getErrorDigest(error: unknown): string | undefined {
  if (!error || typeof error !== 'object') {
    return undefined;
  }

  const digest = (error as { digest?: unknown }).digest;
  return typeof digest === 'string' ? digest : undefined;
}

export function registerNodeInstrumentation(): void {
  installConsoleBridge(logger);

  logger.info(
    {
      source: 'next_instrumentation',
    },
    'next instrumentation registered'
  );
}

export const onNodeRequestError: Instrumentation.onRequestError = async (
  error,
  request,
  context
) => {
  logger.error(
    {
      source: 'next_instrumentation',
      err: normalizeError(error),
      digest: getErrorDigest(error),
      method: request.method,
      path: request.path,
      routerKind: context.routerKind,
      routePath: context.routePath,
      routeType: context.routeType,
      renderSource: context.renderSource,
      revalidateReason: context.revalidateReason,
    },
    'next request error'
  );
};
