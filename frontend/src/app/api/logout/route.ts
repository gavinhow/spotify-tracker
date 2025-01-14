import { cookies } from 'next/headers';
import { redirect } from 'next/navigation';

export async function GET() {
  const cookieStore = await cookies();
  cookieStore.delete('token'); // Set the cookie with secure options
  return redirect('http://localhost:3000'); // Redirect to homepage
}