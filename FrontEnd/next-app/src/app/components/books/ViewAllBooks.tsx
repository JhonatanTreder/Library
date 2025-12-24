"use client"

import booksStyles from '@/app/components/Styles/books/books.module.css'

import ShowFilterBar from "./BookFilterBar"
import ShowPaginationBar from '../PaginationBar'

export default function ViewAllBooks(){
    return(
        <section className={booksStyles.booksContainer}>
            <ShowFilterBar></ShowFilterBar>
            <ShowPaginationBar></ShowPaginationBar>
        </section>
    )
}