import booksStyles from '@/app/components/Styles/books/books.module.css'

import { BookListLayout } from '@/app/components/books/layouts/BookListLayout';
import { BookListTemplateProps } from '@/app/interfaces/template/BookListTemplateProps';

export const BookListTemplate = ({
    books,
    loading,
    error,
    pagination,
    renderBookCard,
    emptyState,
    showFilter = true
}: BookListTemplateProps) => {

    if (loading && books.length === 0) {
        return (
            <BookListLayout showFilter={showFilter}>
                <div className={booksStyles.noBooksContainer}>
                    <h2 className={booksStyles.noBooksMessage}>
                        Carregando livros...
                    </h2>
                    <p className={booksStyles.noBooksSubtitle}>
                        Aguarde enquanto buscamos os livros
                    </p>
                </div>
            </BookListLayout>
        )
    }


    if (error && books.length === 0) {
        return (
            <BookListLayout showFilter={showFilter}>
                <div className={booksStyles.noBooksContainer}>
                    <h2 className={booksStyles.noBooksMessage}>
                        Ocorreu um erro
                    </h2>
                    <p className={booksStyles.noBooksSubtitle}>
                        {error}
                    </p>
                    {emptyState.actionButton}
                </div>
            </BookListLayout>
        );
    }


    if (books.length === 0) {
        return (
            <BookListLayout showFilter={showFilter}>
                <div className={booksStyles.noBooksContainer}>
                    <h2 className={booksStyles.noBooksMessage}>
                        {emptyState.message}
                    </h2>
                    <p className={booksStyles.noBooksSubtitle}>
                        {emptyState.subtitle}
                    </p>
                    {emptyState.actionButton}
                </div>
            </BookListLayout>
        );
    }

    return (
        <BookListLayout
            showFilter={showFilter}
            pagination={pagination}
        >
            <div className={booksStyles.bookList}>
                {books.map(renderBookCard)}
            </div>
        </BookListLayout>
    );
};