"use client"

import { BookListTemplate } from '@/app/components/books/BookListTemplate';
import { BookCard } from '@/app/components/books/BookCard';
import { useBookPagination } from '@/app/components/books/hooks/useBookPagination';
import booksStyles from '@/app/components/Styles/books/books.module.css';

export default function BooksPage() {
    const {
        books,
        loading,
        error,
        pagination,
        handlePageChange,
        handleRedirect
    } = useBookPagination({
        endpoint: '/Book/all',
        defaultPageSize: 10,
        redirectToPage: '/pages/home'
    });

    return (
        <BookListTemplate
            books={books}
            loading={loading}
            error={error}
            pagination={{
                totalItems: pagination.totalItems,
                currentPage: pagination.pageNumber,
                totalPages: pagination.totalPages,
                hasPrevious: pagination.hasPrevious,
                hasNext: pagination.hasNext,
                onPageChange: handlePageChange
            }}
            renderBookCard={(book) => (
                <BookCard key={book.bookId} book={book} />
            )}
            emptyState={{
                message: "Nenhum livro foi encontrado",
                subtitle: "Parece que não existem livros no momento!",
                actionButton: (
                    <button
                        onClick={handleRedirect}
                        className={booksStyles.noBooksAction}
                    >
                        Voltar a página inicial
                    </button>
                )
            }}
        />
    );
}