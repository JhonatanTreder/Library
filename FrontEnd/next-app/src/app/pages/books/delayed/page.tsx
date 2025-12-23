"use client"

import ShowFilterBar from "@/app/components/books/BookFilterBar";
import ShowNavbar from "@/app/components/Navbar";

export default function DelayedBooks() {
    return (
        <section>
            <ShowNavbar></ShowNavbar>
            <ShowFilterBar></ShowFilterBar>
        </section>
    )
}