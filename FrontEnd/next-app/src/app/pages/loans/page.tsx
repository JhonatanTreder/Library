"use client"

import ShowNavbar from "@/app/components/Navbar"
import { useRouter } from "next/navigation"
import { useEffect } from "react"

export default function Loans(){
    const router = useRouter()

    useEffect(() => {
        const token = localStorage.getItem('token')

        if (token === null || token === undefined) {
            localStorage.removeItem('token')
            localStorage.removeItem('refresh-token')
            localStorage.removeItem('toke-expiration-time')

            router.push('/auth/login')
        }
    }, [])

    return(
        <section>
            <ShowNavbar></ShowNavbar>
        </section>
    )
}