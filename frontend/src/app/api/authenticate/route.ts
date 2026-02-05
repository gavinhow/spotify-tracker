import { cookies } from 'next/headers';
import { NextRequest } from 'next/server';
import { redirect } from 'next/navigation';
import { logger } from '@/lib/logger';

export async function GET(request: NextRequest) {
  const startTime = Date.now();
  const searchParams = request.nextUrl.searchParams;
  const code = searchParams.get('code');

  if (!code) {
    logger.warn(
      {
        method: request.method,
        path: request.nextUrl.pathname,
      },
      'Authentication failed - no code provided'
    );
    return redirect('/');
  }

  try {
    const response = await fetch(
      (process.env.INTERNAL_API_URL || process.env.NEXT_PUBLIC_API_URL) +
        `/user/authenticate?code=${code}`
    );

    if (!response.ok) {
      logger.error(
        {
          method: request.method,
          path: request.nextUrl.pathname,
          statusCode: response.status,
          duration: Date.now() - startTime,
        },
        'Authentication failed - backend returned non-OK status'
      );
      return redirect('/');
    }

    const user = (await response.json()) as User;

    const cookieStore = await cookies();
    cookieStore.set('token', JSON.stringify(user), { path: '/', httpOnly: true });

    logger.info(
      {
        method: request.method,
        path: request.nextUrl.pathname,
        statusCode: 302,
        duration: Date.now() - startTime,
        userId: user.id,
      },
      'User authenticated successfully'
    );

    return redirect('/');
  } catch (error) {
    logger.error(
      {
        err: error instanceof Error ? error : new Error(String(error)),
        method: request.method,
        path: request.nextUrl.pathname,
        duration: Date.now() - startTime,
      },
      'Authentication failed with exception'
    );
    return redirect('/');
  }
}