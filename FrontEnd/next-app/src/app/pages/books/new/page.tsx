"use client"

import { BookListTemplate } from '@/app/components/books/BookListTemplate';
import { BookCard } from '@/app/components/books/BookCard';
import { useBookPagination } from '@/app/components/books/hooks/useBookPagination';
import booksStyles from '@/app/components/Styles/books/books.module.css';

export default function NewBooksPage() {
  const {
    books,
    loading,
    error,
    pagination,
    handlePageChange,
    handleRedirect
  } = useBookPagination({
    endpoint: '/Book/new',
    defaultPageSize: 10,
    redirectToPage: '/pages/books'
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
        message: "Nenhum novo livro foi encontrado",
        subtitle: "Parece que não existe livros que foram adicionados recentemente!",
        actionButton: (
          <button 
            onClick={handleRedirect}
            className={booksStyles.noBooksAction}
          >
            Visualizar página de livros
          </button>
        )
      }}
    />
  );
}