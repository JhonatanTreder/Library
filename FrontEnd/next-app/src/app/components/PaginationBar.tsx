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

                    <KeyboardArrowRight sx={{ fontSize: 30 }}/>
                </button>
            </div>
        </section>
    )
}
