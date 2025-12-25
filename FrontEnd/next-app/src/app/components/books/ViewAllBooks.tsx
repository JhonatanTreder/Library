"use client"

import booksStyles from '@/app/components/Styles/books/books.module.css'
import ShowFilterBar from "./BookFilterBar"
import ShowPaginationBar from '@/app/components/PaginationBar'

import { useEffect, useState } from 'react'
import { usePathname, useRouter, useSearchParams } from 'next/navigation'

interface BookReturnDTO {
    bookId: number;
    title: string;
    author: string;
    description: string;
    publisher: string;
    publicationYear: number;
    category: string;
    totalCopies: number;
    availableCopies: number;
    createdAt: string;
}

interface PaginatedDataDTO<T> {
    currentPage: number;
    totalPages: number;
    totalItems: number;
    hasNext: boolean;
    hasPrevious: boolean;
    data: T[];
}

interface RepositoryResponse<T> {
    status: number;
    data?: T;
    message?: string;
}

export default function ViewAllBooks() {
    const router = useRouter()
    const pathname = usePathname()
    const searchParams = useSearchParams()

    const initialPageNumber = Number(searchParams.get('pageNumber')) || 1
    const initialPageSize = Number(searchParams.get('pageSize')) || 10

    const [pageNumber, setPageNumber] = useState(initialPageNumber)
    const [pageSize, setPageSize] = useState(initialPageSize)
    const [totalItems, setTotalItems] = useState(0)
    const [totalPages, setTotalPages] = useState(0)
    const [currentPage, setCurrentPage] = useState(0)
    const [hasPrevious, setHasPrevious] = useState<boolean>(false)
    const [hasNext, setHasNext] = useState<boolean>(false)
    const [books, setBooks] = useState<BookReturnDTO[]>([])

    useEffect(() => {
        const page = Number(searchParams.get('pageNumber')) || 1
        const size = Number(searchParams.get('pageSize')) || 10

        setPageNumber(page)
        setPageSize(size)

    }, [searchParams])

    useEffect(() => {

        fetchGetBooksAsync()

    }, [pageNumber, pageSize])

    async function fetchGetBooksAsync() {
        try {
            const token = localStorage.getItem('token')

            if (!token) {
                localStorage.removeItem('token')
                localStorage.removeItem('refresh-token')
                localStorage.removeItem('token-expiration-time')

                router.push('/auth/login')
                return
            }

            const params = new URLSearchParams()

            params.append('pageNumber', pageNumber.toString())
            params.append('pageSize', pageSize.toString())

            const getBooksURL = `https://localhost:7221/Book/all?${params.toString()}`

            const getBooksRequest = await fetch(getBooksURL, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                }
            })

            if (getBooksRequest.status === 401) {
                router.push('/auth/login')
                return
            }

            const getBooksResponse: RepositoryResponse<PaginatedDataDTO<BookReturnDTO>> = await getBooksRequest.json()
            const responseData = getBooksResponse.data

            if (responseData) {
                setBooks(responseData.data)
                setTotalPages(responseData.totalPages)
                setTotalItems(responseData.totalItems)
                setCurrentPage(responseData.currentPage)
                setHasPrevious(responseData.hasPrevious)
                setHasNext(responseData.hasNext)
            }
            
            else {
                setBooks([])
                setTotalPages(0)
                setTotalItems(0)
                setCurrentPage(0)
                setHasPrevious(false)
                setHasNext(false)
            }
        }
        
        catch (error) {
            console.error('Erro ao buscar livros:', error)
            setBooks([])
            setTotalPages(0)
            setTotalItems(0)
            setCurrentPage(0)
            setHasPrevious(false)
            setHasNext(false)
        }
    }

    const handlePageChange = (page: number) => {
        const params = new URLSearchParams(searchParams.toString())

        params.set('pageNumber', page.toString())
        params.set('pageSize', pageSize.toString())

        router.replace(`${pathname}?${params.toString()}`, { scroll: false })
    }

    return (
        <section className={booksStyles.booksContainer}>
            <ShowFilterBar />
            <ShowPaginationBar
                currentPage={pageNumber}
                totalPages={totalPages}
                hasPrevious={hasPrevious}
                hasNext={hasNext}
                onPageChange={handlePageChange}/>

            <div className={booksStyles.bookList}>
                {books.map(book => (
                    <div key={book.bookId} className={booksStyles.bookCard}>
                        <h3 className={booksStyles.bookTitle}> "{book.title}"</h3>
                        <p className={booksStyles.bookAuthor}>Autor: {book.author}</p>
                        <p className={booksStyles.bookCategory}>Categoria: {book.category}</p>
                        <p className={booksStyles.bookPublisher}> Editora: {book.publisher}</p>
                        <p className={booksStyles.bookYear}> Ano de Publicação: {book.publicationYear}</p>      
                        <p className={booksStyles.availableBooks}>
                            Disponíveis: {book.availableCopies} de {book.totalCopies}
                        </p>
                    </div>
                ))}
            </div>
        </section>
    )
}