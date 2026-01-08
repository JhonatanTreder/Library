"use client"

import { BookListTemplate } from '@/app/components/books/BookListTemplate';
import { BookCard } from '@/app/components/books/BookCard';
import { useBookPagination } from '@/app/components/books/hooks/useBookPagination';
import booksStyles from '@/app/components/Styles/books/books.module.css';

export default function AvailableBooksPage() {
  const {
    books,
    loading,
    error,
    pagination,
    handlePageChange,
    handleRedirect
  } = useBookPagination({
    endpoint: '/Book/available',
    defaultPageSize: 10,
    redirectToPage: '/pages/books/unavailable'
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
        message: "Nenhum livro disponível foi encontrado",
        subtitle: "Parece que todos os livros estão indisponíveis no momento!",
        actionButton: (
          <button 
            onClick={handleRedirect}
            className={booksStyles.noBooksAction}
          >
            Visualizar livros indisponíveis
          </button>
        )
      }}
    />
  );
}