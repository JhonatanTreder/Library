"use client"

import booksStyles from '@/app/components/Styles/books/books.module.css'

import PaginationBar from "@/app/components/PaginationBar"
import { useEffect, useState } from 'react'
import { useRouter, usePathname, useSearchParams } from 'next/navigation'
import ShowFilterBar from '@/app/components/books/BookFilterBar'
import { BookReturnDTO } from '@/app/interfaces/books/BookReturnDTO'
import ShowNavbar from '@/app/components/Navbar'
import { RepositoryResponse } from '@/app/interfaces/responses/RepositoryResponse'
import { PaginatedDataDTO } from '@/app/interfaces/pagination/PaginatedDataDTO'

export default function ShowNewBooks() {
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
        fetchNewBooksAsync()
    }, [pageNumber, pageSize])

    async function fetchNewBooksAsync() {

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

            const getNewBooksURL = `https://localhost:7221/Book/new?${params.toString()}`

            const getNewBooksRequest = await fetch(getNewBooksURL, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/josn',
                    'Authorization': `Bearer${token}`
                }
            })

            const response: RepositoryResponse<PaginatedDataDTO<BookReturnDTO>> = await getNewBooksRequest.json()
            const responseData = response.data

            if (responseData) {
                setBooks(responseData.data)
                setTotalPages(responseData.totalPages)
                setTotalItems(responseData.totalItems)
                setCurrentPage(responseData.currentPage)
                setHasPrevious(responseData.hasPrevious)
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
        <section className={booksStyles.booksSection}>

            {/*Adicionar a classe "booksSection" depois*/}
            {/*(neste componente e no componente padrão da página de livros: books/all)*/}
            <ShowNavbar></ShowNavbar>
            <div className={booksStyles.booksContainer}>
                <ShowFilterBar />
                <PaginationBar
                    currentPage={pageNumber}
                    totalPages={totalPages}
                    hasPrevious={hasPrevious}
                    hasNext={hasNext}
                    onPageChange={handlePageChange} />

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
            </div>
        </section>
    )
}