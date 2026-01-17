"use client"

import cancelEmailChangeStyles from '@/app/auth/cancel-email-change/cancelEmailChange.module.css'

import { useEffect, useState } from "react"
import { useSearchParams } from "next/navigation"

type Status = 'loading' | 'success' | 'error'

export default function ShowEmailCancelationScreen() {

    const [status, setStatus] = useState<Status>('loading')
    const [message, setMessage] = useState('')

    const searchParams = useSearchParams()

    const params = new URLSearchParams(searchParams.toString())

    const userId = searchParams.get('userId')
    const token = searchParams.get('token')
    console.log(token)

    useEffect(() => {
        if (!userId || !token) {
            setStatus('error')
            setMessage('Link inválido.')
            return
        }

        fetchCancelEmailChange()
    }, [])

    async function fetchCancelEmailChange() {
        const cancelEmailChangeURL = `https://localhost:7221/Auth/cancel-email-change?${params.toString()}`

        try {
            const cancelEmailChangeRequest = await fetch(cancelEmailChangeURL, {
                method: 'GET'
            })

            const cancelEmailChangeResponse = await cancelEmailChangeRequest.json()

            console.log(cancelEmailChangeResponse)

            if (cancelEmailChangeRequest.status === 200) {
                setStatus('success')
                setMessage('A alteração do e-mail foi cancelada com sucesso.')
            }

            else {
                setStatus('error')
                setMessage('O link está inválido ou foi expirado.')
            }
        }

        catch {
            setStatus('error')
            setMessage('Erro inesperado ao cancelar o e-mail do usuário.')
        }
    }

    if (status === "loading") {
        return (
            <div className={cancelEmailChangeStyles.container}>
                <p className={cancelEmailChangeStyles.message}>Verificando solicitação...</p>
            </div>
        )
    }

    if (status === "success") {
        return (
            <div className={cancelEmailChangeStyles.container}>
                <h1 className={cancelEmailChangeStyles.title}>Alteração cancelada</h1>
                <p className={cancelEmailChangeStyles.message}>{message}</p>
                <a href="/pages/home" className={cancelEmailChangeStyles.buttonRef}>Ir para a página inicial</a>
            </div>
        )
    }

    return (
        <div className={cancelEmailChangeStyles.container}>
            <h1 className={cancelEmailChangeStyles.title}>Link inválido</h1>
            <p className={cancelEmailChangeStyles.message}>{message}</p>
            <a href="/pages/home" className={cancelEmailChangeStyles.buttonRef}>Ir para a página inicial</a>
        </div>
    )
}