"user client"

import { stringify } from "querystring"
import { useState } from "react"

export function userRegister(){

    const [loading, setLoading] = useState(false)
    const [error, setError] = useState<string | null>(null)
    const [success, setSuccess] = useState(false)

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

            if (!registerRequest.ok){
                console.log(registerRequest)
                const data = await registerRequest.json().catch(() => null);
                console.error('Erro HTTP', registerRequest.status, data);
                return;
            }

            console.log(registerRequest)
            var response = await registerRequest.json();

            if (response['status'] !== 'Ok'){
                console.log('Server Response:')
                console.log(response)
                console.log('-----------------------------------------------------------')

                var responseMessage = response['message']

                setError(responseMessage)
                console.log(response['status'])
                console.log(responseMessage)
            }

            setSuccess(true)

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