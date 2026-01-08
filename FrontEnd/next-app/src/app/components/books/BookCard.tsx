import booksStyles from '@/app/components/Styles/books/books.module.css'
import { BookCardProps } from '@/app/interfaces/books/BookCardProps'

export const BookCard = ({ book, onClick }: BookCardProps) => {
    return (
        <div
            key={book?.bookId}
            className={booksStyles.bookCard}
            onClick={onClick}
        >
            <h3 className={booksStyles.bookTitle}>"{book?.title}"</h3>
            <p className={booksStyles.bookAuthor}>Author: {book?.author}</p>
            <p className={booksStyles.bookYear}>Ano: {book?.publicationYear}</p>
            <p className={booksStyles.bookPublisher}>Editora: {book?.publisher}</p>
            <p className={booksStyles.bookCategory}>Categoria: {book?.category}</p>
            <p className={booksStyles.availableBooks}>
                Disponíveis: {book?.availableCopies} de {book?.totalCopies}
            </p>
        </div>
    )
}