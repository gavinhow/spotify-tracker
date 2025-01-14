import { cookies } from 'next/headers';

export async function getUserCookie(): Promise<User | undefined> {
  const cookiesList = await cookies();
  const spotifyToken = cookiesList.get('token');

  if (!spotifyToken)
    return undefined;

  return JSON.parse(spotifyToken.value) as User;
}