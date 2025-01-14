import type { Metadata } from 'next';
import { Geist, Geist_Mono } from 'next/font/google';
import './globals.css';
import { getUserCookie } from '@/lib/getUserCookie';
import * as React from 'react';
import { headers } from 'next/headers';

const geistSans = Geist({
  variable: '--font-geist-sans',
  subsets: ['latin'],
});

const geistMono = Geist_Mono({
  variable: '--font-geist-mono',
  subsets: ['latin'],
});

export const metadata: Metadata = {
  title: 'Spotify Tracker',
  description: 'Generated by create next app',
};

const  RootLayout = async ({
                                           children,
                                           unauthenticated
                                         }: Readonly<{
  children: React.ReactNode;
  unauthenticated: React.ReactNode;
}>) => {
  const user = await getUserCookie();
  const headersList = await headers();
  const isDemo = headersList.get('is_demo') === 'true';

  return (
    <html lang="en">
    <body
      className={`${geistSans.variable} ${geistMono.variable} antialiased`}
    >
    {user !== undefined || isDemo ? (
      <>
        {children}
      </>) : unauthenticated}
    </body>
    </html>
  );
}
export default RootLayout;
