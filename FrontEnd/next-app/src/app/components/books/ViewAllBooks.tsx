"use client"

import allBooksStyles from '@/app/components/Styles/books/allBooks.module.css'

import ShowFilterBar from "./BookFilterBar"

export default function ViewAllBooks(){
    return(
        <section className={allBooksStyles.booksContainer}>
            <ShowFilterBar></ShowFilterBar>

        </section>
    )
}