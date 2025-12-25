"use client"

import booksStyles from '@/app/components/Styles/books/books.module.css'

import ShowNavbar from "@/app/components/Navbar";
import { useRouter } from "next/navigation";
import { useEffect } from "react";
import ViewAllBooks from '@/app/components/books/ViewAllBooks';

export default function Books() {
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
        <section className={booksStyles.booksSection}>
            <ShowNavbar></ShowNavbar>
            <ViewAllBooks></ViewAllBooks>
        </section>
    )
}