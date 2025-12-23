"use client"

import ShowFilterBar from "@/app/components/books/BookFilterBar";
import ShowNavbar from "@/app/components/Navbar";

export default function UnavailableBooks(){
    return(
        <section>
            <ShowNavbar></ShowNavbar>
            <ShowFilterBar></ShowFilterBar>
        </section>
    )
}