import { redirect } from 'next/navigation';
import { Button } from '@/components/ui/button';
import Image from 'next/image';

export default async function SignIn() {

  return (
    <form
      action={async () => {
        "use server"
        redirect(process.env.NEXT_PUBLIC_API_URL + "/user/login")
      }}
    >
      <Button className="bg-green-600 rounded-full font-bold text-3xl h-20" size="lg" type="submit">
        <Image src={'/Spotify_Icon_RGB_White.png'} alt="Spotify Icon" className="mr-4" width={40} height={40} /> Signin with Spotify</Button>
    </form>
  )
}