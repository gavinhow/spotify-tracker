import { cookies } from 'next/headers';
import { NextRequest } from 'next/server';
import { redirect } from 'next/navigation';

export async function GET(request: NextRequest) {
  const searchParams = request.nextUrl.searchParams
  const code = searchParams.get('code')

  if (!code) {
    return redirect("/");
  }

  const response = await fetch((process.env.INTERNAL_API_URL || process.env.NEXT_PUBLIC_API_URL) + `/user/authenticate?code=${code}`);

  if (!response.ok) {
    return redirect("/");
  }

  const user = await response.json() as User;

  const cookieStore = await cookies();
  cookieStore.set('token', JSON.stringify(user), { path: '/', httpOnly: true }); // Set the cookie with secure options
  return redirect('/'); // Redirect to homepage
}