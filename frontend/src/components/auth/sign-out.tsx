import { signoutAction } from '@/components/auth/signout-action';

interface SignOutProps {
  children?: React.ReactNode;
}

const SignOut = ({children}: SignOutProps) => {
  return <form action={signoutAction}>
    <button className="w-full">
      {children ?? <span>Sign out</span>}
    </button>
  </form>
}

export default SignOut;