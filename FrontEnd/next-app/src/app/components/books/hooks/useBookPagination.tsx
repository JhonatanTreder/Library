"use client"

import { useState, useEffect, useCallback } from "react"
import { useRouter, usePathname, useSearchParams } from "next/navigation"

import {
    SortField,
    SortDirection,
    SortState,
    DEFAULT_SORT_FIELD,
    DEFAULT_SORT_DIRECTION
} from '@/app/types/sortTypes'

import { BookReturnDTO } from "@/app/interfaces/books/BookReturnDTO"
import { PaginatedDataDTO } from "@/app/interfaces/pagination/PaginatedDataDTO"
import { RepositoryResponse } from "@/app/interfaces/responses/RepositoryResponse"
import { UseBookPaginationProps } from "@/app/interfaces/pagination/books/UseBookPaginationProps"
import { UseBookPaginationReturn } from "@/app/interfaces/pagination/books/UseBookPaginationReturn"

export interface UseBookPaginationExtendedProps extends UseBookPaginationProps {
    defaultSortField?: SortField
    defaultSortDirection?: SortDirection
}

export const useBookPagination = ({
    endpoint,
    defaultPageSize = 10,
    redirectToPage,
    defaultSortField = DEFAULT_SORT_FIELD,
    defaultSortDirection = DEFAULT_SORT_DIRECTION
}: UseBookPaginationExtendedProps): UseBookPaginationReturn & {
    sort: SortState
    handleSortChange: (field: SortField, direction: SortDirection) => void
    handleReload?: () => void
} => {

    const router = useRouter()
    const pathname = usePathname()
    const searchParams = useSearchParams()

    const [pageNumber, setPageNumber] = useState(1)
    const [pageSize, setPageSize] = useState(defaultPageSize)
    const [totalItems, setTotalItems] = useState(0)
    const [totalPages, setTotalPages] = useState(0)
    const [hasPrevious, setHasPrevious] = useState<boolean>(false)
    const [hasNext, setHasNext] = useState<boolean>(false)
    const [books, setBooks] = useState<BookReturnDTO[]>([])
    const [loading, setLoading] = useState<boolean>(true)
    const [error, setError] = useState<string | null>(null)

    const [sort, setSort] = useState<SortState>({
        field: defaultSortField,
        direction: defaultSortDirection
    })

    useEffect(() => {
        const currentPageNumber = Number(searchParams.get('pageNumber')) || 1
        const currentPageSize = Number(searchParams.get('pageSize')) || defaultPageSize
        const sortBy = searchParams.get('sortBy') as SortField || defaultSortField
        const sortDirection = searchParams.get('sortDirection') as SortDirection || defaultSortDirection

        setPageNumber(currentPageNumber)
        setPageSize(currentPageSize)
        setSort({
            field: sortBy,
            direction: sortDirection
        })
    }, [searchParams, defaultPageSize, defaultSortField, defaultSortDirection])

    const updateUrlWithPage = useCallback((
        page: number,
        size: number,
        sortField?: SortField,
        sortDir?: SortDirection
    ) => {
        const params = new URLSearchParams(searchParams.toString())

        params.set('pageNumber', page.toString())
        params.set('pageSize', size.toString())

        if (sortField !== undefined) params.set('sortBy', sortField)
        if (sortDir !== undefined) params.set('sortDirection', sortDir)

        router.replace(`${pathname}?${params.toString()}`, { scroll: false })
    }, [router, pathname, searchParams])

    const fetchBooks = async () => {
        try {
            setLoading(true)
            setError(null)

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

            params.append('sortBy', sort.field)
            params.append('sortDirection', sort.direction)

            const url = `https://localhost:7221${endpoint}?${params.toString()}`

            const request = await fetch(url, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                }
            })

            if (request.status === 401) {
                router.push('/auth/login')
                return
            }

            if (!request.ok) {
                throw new Error(`Erro HTTP: ${request.status}`)
            }

            const response: RepositoryResponse<PaginatedDataDTO<BookReturnDTO>> = await request.json()
            const responseData = response.data

            if (responseData) {
                setBooks(responseData.data)
                setTotalPages(responseData.totalPages)
                setTotalItems(responseData.totalItems)

                if (responseData.currentPage !== pageNumber) {
                    setPageNumber(responseData.currentPage)
                    updateUrlWithPage(responseData.currentPage, pageSize, sort.field, sort.direction)
                }

                setHasPrevious(responseData.currentPage > 1)
                setHasNext(responseData.currentPage < responseData.totalPages)
            } else {
                setBooks([])
                setTotalPages(0)
                setTotalItems(0)
                setHasPrevious(false)
                setHasNext(false)
            }
        } catch (error) {
            console.error(`Erro ao buscar por livros: ${error}`)
            setError('Não foi possível carregar os livros. Tente novamente.')
            setBooks([])
            setTotalPages(0)
            setTotalItems(0)
            setHasPrevious(false)
            setHasNext(false)
        } finally {
            setLoading(false)
        }
    }

    useEffect(() => {
        fetchBooks()
    }, [pageNumber, pageSize, sort.field, sort.direction])

    const handlePageChange = useCallback((page: number) => {
        updateUrlWithPage(page, pageSize)
    }, [updateUrlWithPage, pageSize])

    const handlePageSizeChange = useCallback((size: number) => {
        updateUrlWithPage(1, size)
        setPageSize(size)
    }, [updateUrlWithPage])

    const handleRedirect = useCallback(() => {
        if (redirectToPage) {
            router.push(redirectToPage)
        } else {
            router.push('/pages/books')
        }
    }, [router, redirectToPage])

    const handleSortChange = useCallback((field: SortField, direction: SortDirection) => {
        setSort({ field, direction })
        updateUrlWithPage(1, pageSize, field, direction)
        setPageNumber(1)
    }, [updateUrlWithPage, pageSize])

    const handleReload = useCallback(() => {
        fetchBooks()
    }, [])

    return {
        books,
        loading,
        error,
        pagination: {
            pageNumber,
            pageSize,
            totalItems,
            totalPages,
            hasPrevious,
            hasNext
        },
        sort,
        fetchBooks,
        handlePageChange,
        handlePageSizeChange,
        handleRedirect,
        handleSortChange,
        handleReload
    }
}