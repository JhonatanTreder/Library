"use client"

// app/auth/login/hooks/useLogin.ts
import { useState } from 'react';
import { useRouter } from 'next/navigation';

interface LoginData {
  email: string;
  password: string;
}

export function useLogin() {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const router = useRouter();

  const loginUser = async (formData: LoginData) => {
    setLoading(true);
    setError(null);

    try {
      const loginURL = 'https://localhost:7221/Auth/login'
      const response = await fetch(loginURL, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(formData),
      });

      if (!response.ok) {
        console.error(response)
        localStorage.removeItem('token')
        localStorage.removeItem('refresh-token')
        localStorage.removeItem('toke-expiration-time')
        return;
      }

      const loginResponse = await response.json();

      if (loginResponse.data.token) {

        localStorage.setItem('token', loginResponse.data.token);
        localStorage.setItem('refresh-token', loginResponse.data.refreshToken)
        localStorage.setItem('token-expiration-time', loginResponse.data.expiration)

        router.push('/pages/home')
      }

      return loginResponse;
    }

    catch (err) {

      setError(err instanceof Error ? err.message : 'Erro desconhecido');
      console.error('Erro no login:', err);
    }

    finally {
      setLoading(false);
    }
  };

  return { loginUser, loading, error };
}