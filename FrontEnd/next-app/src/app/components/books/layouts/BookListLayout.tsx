import { ReactNode } from 'react';

import ShowNavbar from '@/app/components/Navbar';
import ShowFilterBar from '@/app/components/books/BookFilterBar';
import PaginationBar from '@/app/components/PaginationBar';
import booksStyles from '@/app/components/Styles/books/books.module.css';

import { BookListLayoutProps } from '@/app/interfaces/layouts/BookListLayoutProps';

export const BookListLayout = ({
  children,
  showFilter = true,
  showPaginationTop = true,
  showPaginationBottom = true,
  pagination,
  ordernation
}: BookListLayoutProps) => {
  
  console.log('BookListLayout - ordernation:', ordernation)
  return (
    <section className={booksStyles.booksSection}>
      <ShowNavbar />
      <div className={booksStyles.booksContainer}>
        {showFilter && <ShowFilterBar />}

        {showPaginationTop && pagination && pagination.totalPages > 1 && (
          <PaginationBar
            key={`top-${ordernation?.sortField}-${ordernation?.sortDirection}`}
            currentPage={pagination.currentPage}
            totalPages={pagination.totalPages}
            hasPrevious={pagination.hasPrevious}
            hasNext={pagination.hasNext}
            onPageChange={pagination.onPageChange}
            ordernation={ordernation}
          />
        )}

        {children}

        {showPaginationBottom && pagination && pagination.totalPages > 1 && (
          <PaginationBar
            key={`bottom-${ordernation?.sortField}-${ordernation?.sortDirection}`}
            currentPage={pagination.currentPage}
            totalPages={pagination.totalPages}
            hasPrevious={pagination.hasPrevious}
            hasNext={pagination.hasNext}
            onPageChange={pagination.onPageChange}
            ordernation={ordernation}
          />
        )}
      </div>
    </section>
  );
};