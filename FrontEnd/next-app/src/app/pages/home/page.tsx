"use client"

import homeStyles from '@/app/pages/styles/home/homeStyles.module.css'

//Componentes
import ShowNavbar from '@/app/components/Navbar'
import ShowHeaderSection from '@/app/components/home/HeaderSection'
import ShowLibraryStats from '@/app/components/home/LibraryStats'

import { useRouter } from 'next/navigation'
import { useEffect } from 'react'

export default function Home() {

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

    return (
        <section className={homeStyles.homeSection}>
            <ShowNavbar></ShowNavbar>
            <ShowHeaderSection></ShowHeaderSection>
            <ShowLibraryStats></ShowLibraryStats>
        </section>
    )
}
