import { cookies } from 'next/headers';
import { redirect } from 'next/navigation';
import { logger } from '@/lib/logger';

export async function GET() {
  const cookieStore = await cookies();
  const tokenCookie = cookieStore.get('token');

  // Extract user ID from token if available for logging
  let userId: string | undefined;
  try {
    if (tokenCookie?.value) {
      const user = JSON.parse(tokenCookie.value);
      userId = user.id;
    }
  } catch {
    // Ignore parsing errors
  }

  cookieStore.delete('token');

  logger.info(
    {
      userId,
      action: 'logout',
    },
    'User logged out'
  );

  return redirect('/');
}