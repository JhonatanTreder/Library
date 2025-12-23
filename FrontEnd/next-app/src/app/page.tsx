"use client"

import RegisterForm from '@/app/auth/register/page';
import ShowValidationScreen from '@/app/auth/validation/page';
import HomePage from '@/app/pages/home/page';
import LoginForm from '@/app/auth/login/page'
import { useRouter } from 'next/navigation';

export default function Home() {

  const token = localStorage.getItem('token')
  const router = useRouter()

  if (token === null || token === undefined){
    router.push('/atuh/login')
  }

  return (
    <HomePage></HomePage>
  );
}
