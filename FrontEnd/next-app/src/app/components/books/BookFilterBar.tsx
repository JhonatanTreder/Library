import bookFilterBarStyles from '@/app/components/Styles/books/bookFilterBar.module.css'

import { usePathname, useRouter } from 'next/navigation'
import { useEffect, useState } from 'react'

export default function ShowFilterBar() {

    const router = useRouter()
    const pathname = usePathname()
    const [activeItem, setActiveItem] = useState('')

    useEffect(() => {

        if (pathname.endsWith('books')) {
            setActiveItem('patternType')
        }

        else if (pathname.endsWith('/new')) {
            setActiveItem('newType')
        }

        else if (pathname.endsWith('/recent')) {
            setActiveItem('recentType')
        }

        else if (pathname.endsWith('/delayed')) {
            setActiveItem('delayedType')
        }

        else if (pathname.endsWith('/available')) {
            setActiveItem('availableType')
        }

        else if (pathname.endsWith('/unavailable')) {
            setActiveItem('unavailableType')
        }

    }, [pathname])

    const handleViewBooksPage = () => {
        router.push('/pages/books')
    }

    const handleViewNewBooksPage = () => {
        router.push('/pages/books/new')
    }

    const handleViewRecentBooksPage = () => {
        router.push('/pages/books/recent')
    }

    const handleViewDelayedBooksPage = () =>{
        router.push('/pages/books/delayed')
    }

    const handleViewAvailableBooksPage = () =>{
        router.push('/pages/books/available')
    }

    const handleViewUnavailableBooksPage = () =>{
        router.push('/pages/books/unavailable')
    }

    return (
        <section className={bookFilterBarStyles.container}>
            <div className={bookFilterBarStyles.bookNavigationBar}>

                <div className={bookFilterBarStyles.navigationTypes}>
                    <div
                        className={`
                            ${bookFilterBarStyles.navigationType}
                            ${activeItem === 'patternType' ? bookFilterBarStyles.navigationTypeActive : ''}
                        `}
                        onClick={handleViewBooksPage}>
                        Diversos
                    </div>
                </div>

                <div className={bookFilterBarStyles.navigationTypes}>
                    <div
                        className={`
                            ${bookFilterBarStyles.navigationType}
                            ${activeItem === 'newType' ? bookFilterBarStyles.navigationTypeActive : ''}
                        `}
                        onClick={handleViewNewBooksPage}>
                        Novos
                    </div>
                </div>

                <div className={bookFilterBarStyles.navigationTypes}>
                    <div
                        className={`
                            ${bookFilterBarStyles.navigationType}
                            ${activeItem === 'recentType' ? bookFilterBarStyles.navigationTypeActive : ''}
                        `}
                        onClick={handleViewRecentBooksPage}>
                        Recentes
                    </div>
                </div>

                <div className={bookFilterBarStyles.navigationTypes}>
                    <div
                        className={`
                            ${bookFilterBarStyles.navigationType}
                            ${activeItem === 'delayedType' ? bookFilterBarStyles.navigationTypeActive : ''}
                        `}
                        onClick={handleViewDelayedBooksPage}>
                        Emprestados
                    </div>
                </div>

                <div className={bookFilterBarStyles.navigationTypes}>
                    <div
                        className={`
                            ${bookFilterBarStyles.navigationType}
                            ${activeItem === 'availableType' ? bookFilterBarStyles.navigationTypeActive : ''}
                        `}
                        onClick={handleViewAvailableBooksPage}>
                        Disponíveis
                    </div>
                </div>

                <div className={bookFilterBarStyles.navigationTypes}>
                    <div
                        className={`
                            ${bookFilterBarStyles.navigationType}
                            ${activeItem === 'unavailableType' ? bookFilterBarStyles.navigationTypeActive : ''}
                        `}
                        onClick={handleViewUnavailableBooksPage}>
                        Indisponíveis
                    </div>
                </div>
            </div>
        </section>
    )
}