"use client"

import { useState } from "react"
import { useRouter } from "next/navigation"

export function useUserRegister(){

    const [loading, setLoading] = useState(false)
    const [error, setError] = useState<string | null>(null)
    const [success, setSuccess] = useState(false)

    const router = useRouter()

    async function registerUser(formData:{

        name: string
        email: string
        password: string
        matriculates: string
    })
    {
        setLoading(true)
        setError(null)
        setSuccess(false)

        try{
            const registerUrl = `https://localhost:7221/Auth/register`

            const registerRequest = await fetch(registerUrl, {
                method: 'POST',
                headers:{
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(formData)
            })

            var response = await registerRequest.json();
            var responseMessage = response['message']

            if (response['status'] !== 'Ok'){

                setError(responseMessage)
                console.log(response)
                console.log(responseMessage)
            }

            else{
                console.log(response['status'])
                console.log(responseMessage)

                const sendEmailConfirmationURl = `https://localhost:7221/Auth/email-confirmation`

                console.log(formData.email)

                const emailConfirmationRequest = await fetch(sendEmailConfirmationURl, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify(formData.email)
                })

                const emailConfirmationResponse = await emailConfirmationRequest.json()

                if (emailConfirmationResponse['status'] !== 'Ok'){
                    console.log('a')
                    setError(responseMessage)
                }

                else{
                    setSuccess(true)
                    console.log('b')
                    console.log(emailConfirmationResponse)
                    console.log(emailConfirmationResponse['message'])
                    sessionStorage.setItem('user-email', formData.email)

                    router.push('/auth/validation/')
                }
            }

            return response
        }
        catch (error: any){
            setError(error.message || 'Erro inesperado')
        }
        finally{
            setLoading(false)
        }
    }

    return {registerUser, loading, error, success}
}