"use client"

import paginationBarStyles from '@/app/components/Styles/paginationBar.module.css'
import { KeyboardArrowLeft, KeyboardArrowRight } from "@mui/icons-material"
import { useState, useEffect } from "react"
import { usePathname, useRouter, useSearchParams } from "next/navigation"

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

interface PaginationParameters {
    pageNumber: number;
    pageSize: number;
}

interface BookFilterDTO {
    // Adicione os campos do filtro conforme sua API
    title?: string;
    author?: string;
    category?: string;
}

export default function ShowPaginationBar() {
    const router = useRouter()
    const pathname = usePathname()
    const searchParams = useSearchParams()

    const [pageNumber, setPageNumber] = useState(1)
    const [pageSize, setPageSize] = useState(10)
    const [totalPages, setTotalPages] = useState(0)
    const [totalItems, setTotalItems] = useState(0)
    const [books, setBooks] = useState<BookReturnDTO[]>([])
    const [hasPrevious, setHasPrevious] = useState<boolean>(false)
    const [hasNext, setHasNext] = useState<boolean>(false)

    useEffect(() => {
        const page = Number(searchParams.get('pageNumber')) || 1
        const size = Number(searchParams.get('pageSize')) || 10

        setPageNumber(page)
        setPageSize(size)

    }, [searchParams])

    useEffect(() => {
        fetchBooksAsync()
    }, [pageNumber, pageSize])

    async function fetchBooksAsync() {
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

            const response: RepositoryResponse<PaginatedDataDTO<BookReturnDTO>> = await getBooksRequest.json()

            if (response.data) {
                setBooks(response.data.data || [])
                setTotalItems(response.data.totalItems)
                setTotalPages(response.data.totalPages)
                setHasPrevious(response.data.hasPrevious)
                setHasNext(response.data.hasNext)
            }

            else {
                setBooks([])
                setTotalPages(0)
                setTotalItems(0)
                setHasPrevious(false)
                setHasNext(false)
            }
        }

        catch (error) {
            console.error('Erro ao buscar livros:', error)
        }
    }

    const navigateToPage = (newPage: number) => {
        if (newPage < 1 || newPage > totalPages) return

        const params = new URLSearchParams(searchParams.toString())

        params.set('pageNumber', newPage.toString())
        params.set('pageSize', pageSize.toString())

        router.replace(`${pathname}?${params.toString()}`, { scroll: false })

    }

    const goToPreviousPage = () => {
        if (!hasPrevious) return
        navigateToPage(pageNumber - 1)
    }

    const goToNextPage = () => {
        if (!hasNext) return
        navigateToPage(pageNumber + 1)
    }

    const goToPage = (page: number) => {
        if (page === pageNumber) return
        navigateToPage(page)
    }

    const getPageNumbers = () => {
        const pages = []
        const maxVisiblePages = 5

        if (totalPages <= maxVisiblePages) {

            for (let i = 1; i <= totalPages; i++) {
                pages.push(i)
            }
        }

        else {
            let start = Math.max(1, pageNumber - 2)
            let end = Math.min(totalPages, start + maxVisiblePages - 1)

            if (end - start + 1 < maxVisiblePages) {
                start = Math.max(1, end - maxVisiblePages + 1)
            }

            if (start > 1) {
                pages.push(1)
                if (start > 2) pages.push('...')
            }

            for (let i = start; i <= end; i++) {
                pages.push(i)
            }

            if (end < totalPages) {
                if (end < totalPages - 1) pages.push('...')
                pages.push(totalPages)
            }
        }

        return pages
    }

    const pageNumbers = getPageNumbers()

    return (
        <section className={paginationBarStyles.paginationBarContainer}>
            <div className={paginationBarStyles.navigationButtons}>
                <button
                    className={`
                        ${!hasPrevious ? paginationBarStyles.arrowButtonDisabled : paginationBarStyles.arrowButton}
                    `}
                    onClick={goToPreviousPage}
                    disabled={!hasPrevious}
                >
                    <KeyboardArrowLeft sx={{ fontSize: 30 }} />
                </button>

                <div className={paginationBarStyles.numericalButtons}>
                    {getPageNumbers().map((page, index) => (
                        <button
                            key={index}
                            onClick={() => typeof page === 'number' ? goToPage(page) : null}
                            disabled={page === '...'}
                            className={`
                                ${paginationBarStyles.pageNumber}
                                ${page === pageNumber ? paginationBarStyles.activedButton : ''}
                                ${page === '...' ? paginationBarStyles.disabledButton : ''}`
                            }>
                            {page}
                        </button>
                    ))}
                </div>

                <button
                    className={`
                        ${!hasNext ? paginationBarStyles.arrowButtonDisabled : paginationBarStyles.arrowButton}
                    `}
                    onClick={goToNextPage}
                    disabled={!hasNext}
                >
                    <KeyboardArrowRight sx={{ fontSize: 30 }} />
                </button>
            </div>
        </section>
    )
}