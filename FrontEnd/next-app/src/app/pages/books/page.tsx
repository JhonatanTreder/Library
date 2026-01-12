"use client"

import { BookListTemplate } from '@/app/components/books/BookListTemplate';
import { BookCard } from '@/app/components/books/BookCard';
import { useBookPagination } from '@/app/components/books/hooks/useBookPagination';
import booksStyles from '@/app/components/Styles/books/books.module.css';

export default function BooksPage() {
  const hook = useBookPagination({
    endpoint: '/Book/all',
    defaultPageSize: 10,
    redirectToPage: '/pages/home'
  });

  console.log('Página - hook completo:', hook);
  console.log('Página - hook.ordernation:', hook.ordernation);
  console.log('Página - hook.ordernation?.sortField:', hook.ordernation?.sortField);
  console.log('Página - hook.ordernation?.sortDirection:', hook.ordernation?.sortDirection);

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
      ordernation={{
        sortField: hook.ordernation.sortField,
        sortDirection: hook.ordernation.sortDirection,
        handleSortFieldChange: hook.ordernation.handleSortFieldChange,
        handleSortDirectionChange: hook.ordernation.handleSortDirectionChange
      }}
      renderBookCard={(book) => (
        <BookCard key={book.bookId} book={book} />
      )}
      emptyState={{
        message: "Nenhum livro foi encontrado",
        subtitle: "Parece que não existem livros no momento",
        actionButton: (
          <button 
            onClick={hook.handleRedirect}
            className={booksStyles.noBooksAction}
          >
            Voltar a página inicial
          </button>
        )
      }}
    />
  );
}