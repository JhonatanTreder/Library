"use client"

import styles from '@/app/components/Styles/paginationBar.module.css'
import { KeyboardArrowLeft, KeyboardArrowRight } from "@mui/icons-material"

type PageItem = number | '...'

interface PaginationBarProps {
    currentPage: number
    totalPages: number
    hasPrevious: boolean
    hasNext: boolean
    onPageChange: (page: number) => void
}

export default function PaginationBar({
    currentPage,
    totalPages,
    hasPrevious,
    hasNext,
    onPageChange
}: PaginationBarProps) {

    const getPageNumbers = (): PageItem[] => {
        const pages: PageItem[] = []
        const maxVisiblePages = 5

        if (totalPages <= maxVisiblePages) {
            for (let i = 1; i <= totalPages; i++) {
                pages.push(i)
            }
        } else {
            let start = Math.max(1, currentPage - 2)
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

    const pages = getPageNumbers()

    return (
        <section className={styles.paginationBarContainer}>
            <div className={styles.navigationButtons}>
                <button
                    className={!hasPrevious ? styles.arrowButtonDisabled : styles.arrowButton}
                    disabled={!hasPrevious}
                    onClick={() => onPageChange(currentPage - 1)}>

                    <KeyboardArrowLeft sx={{ fontSize: 30 }} />
                </button>

                <div className={styles.numericalButtons}>
                    {pages.map((page, index) => (
                        <button
                            key={index}
                            disabled={page === '...'}
                            onClick={() => typeof page === 'number' && onPageChange(page)}
                            className={`
                                ${styles.pageNumber}
                                ${page === currentPage ? styles.activedButton : ''}
                                ${page === '...' ? styles.disabledButton : ''}
                            `}>
                            {page}
                        </button>
                    ))}
                </div>

                <button
                    className={!hasNext ? styles.arrowButtonDisabled : styles.arrowButton}
                    disabled={!hasNext}
                    onClick={() => onPageChange(currentPage + 1)}>

                    <KeyboardArrowRight sx={{ fontSize: 30 }} />
                </button>
            </div>
            <div className={styles.ordernationSection}>
                <p>Ordernar por:</p>
                <select name='sortOptions' className={styles.sortOptions}>
                    <option value='title' className={styles.sortOption}>Título</option>
                    <option value='author' className={styles.sortOption}>Author</option>
                    <option value='category' className={styles.sortOption}>Categoria</option>
                    <option value='publisher' className={styles.sortOption}>Editora</option>
                    <option value='totalCopies' className={styles.sortOption}>Total de Cópias</option>
                    <option value='availableCopies' className={styles.sortOption}>Cópias Disponíveis</option>
                    <option value='publicationYear' className={styles.sortOption}>Ano de Publicação</option>
                </select>

                <select name='sortDirection' className={styles.sortOptions}>
                    <option value='asc' className={styles.sortOption}>Ascendente</option>
                    <option value='desc' className={styles.sortOption}>Decrescente</option>
                </select>
            </div>
        </section>
    )
}
