import { cookies } from 'next/headers';
import { NextRequest } from 'next/server';
import { redirect } from 'next/navigation';

export async function GET(request: NextRequest) {
  const searchParams = request.nextUrl.searchParams
  const code = searchParams.get('code')

  if (!code) {
    return redirect("http://localhost:3000");
  }

  const response = await fetch(process.env.NEXT_PUBLIC_API_URL + `/user/authenticate?code=${code}`);

  if (!response.ok) {
    return redirect("http://localhost:3000");
  }

  const user = await response.json() as User;

  const cookieStore = await cookies();
  cookieStore.set('token', JSON.stringify(user), { path: '/', httpOnly: true }); // Set the cookie with secure options
  return redirect('http://localhost:3000'); // Redirect to homepage
}