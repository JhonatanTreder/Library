"use client"

import { BookListTemplate } from '@/app/components/books/BookListTemplate';
import { BookCard } from '@/app/components/books/BookCard';
import { useBookPagination } from '@/app/components/books/hooks/useBookPagination';
import booksStyles from '@/app/components/Styles/books/books.module.css';

export default function BorrowedBooksPage() {
  const hook = useBookPagination({
    endpoint: '/Book/borrowed',
    defaultPageSize: 10,
    redirectToPage: '/pages/books/available'
  });

  console.log(hook.sort.field)
  console.log(hook.sort.direction)

  return (
    <BookListTemplate
      books={hook.books}
      loading={hook.loading}
      error={hook.error}
      pagination={{
        totalItems: hook.pagination.totalItems,
        currentPage: hook.pagination.pageNumber,
        totalPages: hook.pagination.totalPages,
        hasPrevious: hook.pagination.hasPrevious,
        hasNext: hook.pagination.hasNext,
        onPageChange: hook.handlePageChange
      }}
      renderBookCard={(book) => (
        <BookCard key={book.bookId} book={book} />
      )}
      emptyState={{
        message: "Nenhum livro emprestado foi encontrado",
        subtitle: "Parece que todos os livros estão disponíveis no momento!",
        actionButton: (
          <button 
            onClick={hook.handleRedirect}
            className={booksStyles.noBooksAction}
          >
            Visualizar livros disponíveis
          </button>
        )
      }}
    />
  );
}