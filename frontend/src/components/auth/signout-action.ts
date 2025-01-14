"use server";

import { cookies } from 'next/headers';
import { redirect } from 'next/navigation';

export async function signoutAction() {
  (await cookies()).delete('token')
  redirect("/")
}
