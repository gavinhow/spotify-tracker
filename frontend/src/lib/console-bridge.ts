import type { Logger } from './logger';

const CONSOLE_BRIDGE_INSTALLED = Symbol.for('spotifyStatistics.consoleBridgeInstalled');

type ConsoleLevel = 'debug' | 'info' | 'warn' | 'error';
type ConsoleMethod = (...args: unknown[]) => void;

function serializeConsoleArg(value: unknown): unknown {
  if (value instanceof Error) {
    return {
      type: value.name,
      message: value.message,
      stack_trace: value.stack,
    };
  }

  return value;
}

function toError(value: unknown): Error | undefined {
  if (value instanceof Error) {
    return value;
  }

  return undefined;
}

function createPatchedConsoleMethod(
  logger: Logger,
  level: ConsoleLevel,
  originalMethod: ConsoleMethod
): ConsoleMethod {
  return (...args: unknown[]) => {
    try {
      const message = getMessage(level, args);
      const err = args.map(toError).find((value) => value !== undefined);

      const context: Record<string, unknown> = {
        source: 'next_console',
      };

      if (err) {
        context.err = err;
      }

      if (args.length > 1 || typeof args[0] !== 'string') {
        context.consoleArgs = args.map(serializeConsoleArg);
      }

      logger[level](context, message);
    } catch (bridgeError) {
      // Fall back to the original console method if bridge logging fails.
      originalMethod(...args);
      originalMethod('console bridge failed', bridgeError);
    }
  };
}

function getMessage(level: ConsoleLevel, args: unknown[]): string {
  if (args.length === 0) {
    return `console.${level}`;
  }

  const firstArg = args[0];
  if (typeof firstArg === 'string') {
    return firstArg;
  }

  if (firstArg instanceof Error) {
    return firstArg.message;
  }

  return `console.${level}`;
}

export function installConsoleBridge(logger: Logger): void {
  const globalState = globalThis as typeof globalThis & {
    [CONSOLE_BRIDGE_INSTALLED]?: boolean;
  };

  if (globalState[CONSOLE_BRIDGE_INSTALLED]) {
    return;
  }

  const originalMethods: Record<ConsoleLevel, ConsoleMethod> = {
    debug: console.debug.bind(console),
    info: console.info.bind(console),
    warn: console.warn.bind(console),
    error: console.error.bind(console),
  };

  console.debug = createPatchedConsoleMethod(logger, 'debug', originalMethods.debug);
  console.info = createPatchedConsoleMethod(logger, 'info', originalMethods.info);
  console.warn = createPatchedConsoleMethod(logger, 'warn', originalMethods.warn);
  console.error = createPatchedConsoleMethod(logger, 'error', originalMethods.error);

  globalState[CONSOLE_BRIDGE_INSTALLED] = true;
}
