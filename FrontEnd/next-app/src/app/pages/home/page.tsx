"use client"

import homeStyles from '@/app/pages/styles/home/homeStyles.module.css'

//Componentes
import ShowNavbar from '@/app/components/Navbar'
import ShowHeaderSection from '@/app/components/home/HeaderSection'
import ShowLibraryStats from '@/app/components/home/LibraryStats'

export default function Home(){
    return (
        <section className={homeStyles.homeSection}>
            <ShowNavbar></ShowNavbar>
            <ShowHeaderSection></ShowHeaderSection>
            <ShowLibraryStats></ShowLibraryStats>
        </section>
    )
}
