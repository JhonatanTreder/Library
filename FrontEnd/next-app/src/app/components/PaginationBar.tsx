"use client"

import { useState, ChangeEvent } from 'react'
import styles from '@/app/components/Styles/paginationBar.module.css'
import { KeyboardArrowLeft, KeyboardArrowRight } from "@mui/icons-material"
import { DEFAULT_SORT_DIRECTION, DEFAULT_SORT_FIELD, SortField, SortDirection } from '../types/sortTypes'

type PageItem = number | '...'

interface PaginationBarProps {
    currentPage: number
    totalPages: number
    hasPrevious: boolean
    hasNext: boolean
    onPageChange: (page: number) => void

    ordernation?: {
        sortField: SortField;
        sortDirection: SortDirection;
        handleSortFieldChange: (element: React.ChangeEvent<HTMLSelectElement>) => void;
        handleSortDirectionChange: (element: React.ChangeEvent<HTMLSelectElement>) => void;
    }
}

export default function PaginationBar({
    currentPage,
    totalPages,
    hasPrevious,
    hasNext,
    onPageChange,
    ordernation
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

    const handleSortFieldChange = (e: ChangeEvent<HTMLSelectElement>) => {
        console.log('Campo selecionado:', e.target.value)
        if (ordernation?.handleSortFieldChange) {
            ordernation.handleSortFieldChange(e)
        }
    }

    const handleSortDirectionChange = (e: ChangeEvent<HTMLSelectElement>) => {
        console.log('Direção selecionada:', e.target.value)
        if (ordernation?.handleSortDirectionChange) {
            ordernation.handleSortDirectionChange(e)
        }
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
                <p>Ordenar por:</p>

                <select
                    name='sortOptions'
                    className={styles.sortOptions}
                    value={ordernation?.sortField || DEFAULT_SORT_FIELD}
                    onChange={handleSortFieldChange}
                >
                    <option value='title'>Título</option>
                    <option value='author'>Autor</option>
                    <option value='category'>Categoria</option>
                    <option value='publisher'>Editora</option>
                    <option value='totalCopies'>Total de Cópias</option>
                    <option value='availableCopies'>Cópias Disponíveis</option>
                    <option value='publicationYear'>Ano de Publicação</option>
                </select>

                <select
                    name='sortDirection'
                    className={styles.sortOptions}
                    value={ordernation?.sortDirection || DEFAULT_SORT_DIRECTION}
                    onChange={handleSortDirectionChange}
                >
                    <option value='asc'>Ascendente</option>
                    <option value='desc'>Decrescente</option>
                </select>
            </div>
        </section>
    )
}