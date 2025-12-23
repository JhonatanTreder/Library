"use client"

import ShowNavbar from "@/app/components/Navbar"
import { useEffect, useState } from "react"
import { useRouter } from "next/navigation"
import newBookStyles from '@/app/pages/books/new/styles/newBooks.module.css'
import ShowFilterBar from "@/app/components/books/BookFilterBar"

export default function NewBooks() {
    const router = useRouter()
    const [books, setBooks] = useState<Book[]>([])
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState<string | null>(null)

    interface Book {
        bookId: number;
        title: string;
        author: string;
        description: string;
        publisher: string;
        category: string;
        publicationYear: number;
        totalCopies: number;
        availableCopies: number;
        createdAt: string;
    }

    useEffect(() => {
        fetchNewBooks()
    }, [])

    async function fetchNewBooks() {
        try {
            const token = localStorage.getItem('token')
            if (!token) {
                router.push('/auth/login')
                return
            }

            const response = await fetch('https://localhost:7221/Book/recents', {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                }
            })

            if (response.status === 401) {
                router.push('/auth/login')
                return
            }

            const data = await response.json()
            setBooks(data.data || data || [])

        }

        catch (error) {
            setError('Erro ao carregar livros')
            console.error(error)

        }

        finally {
            setLoading(false)
        }
    }

    const formatDate = (dateString: string) => {

        return new Date(dateString).toLocaleDateString('pt-BR')
    }

    if (loading) return (
        <div>
            <ShowNavbar />
            <div className={newBookStyles.loadingContainer}>Carregando...</div>
        </div>
    )

    if (error) return (
        <div>
            <ShowNavbar />
            <div className={newBookStyles.errorContainer}>{error}</div>
        </div>
    )

    return (
        <section>

            <ShowNavbar></ShowNavbar>
            <ShowFilterBar></ShowFilterBar>

            {books.length === 0 ? (
                <div className={newBookStyles.emptyContainer}>
                    <p>Nenhum livro novo encontrado.</p>
                </div>
            ) : (
                <section>
                    {books.map((book) => (
                        <div>
                            <div>
                                {book.title}
                            </div>
                        </div>
                    ))}
                </section>
            )}
        </section>
    )
}