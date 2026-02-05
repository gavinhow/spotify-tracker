import type { Instrumentation } from 'next';

export async function register(): Promise<void> {
  if (process.env.NEXT_RUNTIME !== 'nodejs') {
    return;
  }

  const { registerNodeInstrumentation } = await import('./instrumentation.node');
  registerNodeInstrumentation();
}

export const onRequestError: Instrumentation.onRequestError = async (
  error,
  request,
  context
) => {
  if (process.env.NEXT_RUNTIME !== 'nodejs') {
    return;
  }

  const { onNodeRequestError } = await import('./instrumentation.node');
  await onNodeRequestError(error, request, context);
};
