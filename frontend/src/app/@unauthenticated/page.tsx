import React from 'react';
import SignIn from '@/components/auth/sign-in';
import Link from 'next/link';
import { Button } from '@/components/ui/button';
import { LoginForm } from '@/components/login-form';

const LoginPage = () => {
  return (
    <div className="flex min-h-svh flex-col items-center bg-cover bg-center  justify-center gap-6 bg-[url('/background.jpg')] p-6 md:p-10">
      <div className="flex w-full max-w-sm flex-col gap-6">
        <LoginForm />
      </div>
    </div>
  )

  return (
    <div className="h-screen w-screen flex flex-col space-y-4 items-center justify-center">
      <SignIn  />
      <Link  href="/demo">
        <Button size="lg">
        Demo
        </Button>
      </Link>
    </div>
  );
};

export default LoginPage;
