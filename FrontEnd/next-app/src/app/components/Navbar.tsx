"use client"

import { useEffect, useState } from 'react'
import { useRouter } from 'next/navigation'
import { usePathname } from 'next/navigation'

import Image from 'next/image'
import navbarStyles from '@/app/components/Styles/navbar.module.css'
import schoolAndBook from '@/app/Images/home/white-library-logo.png'

// Icons
import SearchIcon from '@mui/icons-material/Search'
import HomeIcon from '@mui/icons-material/Home'
import BookIcon from '@mui/icons-material/AutoStories'
import EventsIcon from '@mui/icons-material/Event'
import LoansIcon from '@mui/icons-material/AssignmentReturn'
import FavoritsIcon from '@mui/icons-material/Bookmark'
import NotificationsIcon from '@mui/icons-material/Notifications'
import ProfileIcon from '@mui/icons-material/Person'

export default function ShowNavbar() {

    const router = useRouter()
    const pathname = usePathname()
    const [activeItem, setActiveItem] = useState('')

    useEffect(() => {

        const bookRoutes = ['books', 'new', 'recent', 'available', 'unavailable']
        const isBookRoute = bookRoutes.some(route => pathname.includes(route))

        if (pathname.endsWith('home')) {
            setActiveItem('home')
        }

        else if (pathname.endsWith('books') || isBookRoute) {
            setActiveItem('books')
        }

        else if (pathname.endsWith('events')) {
            setActiveItem('events')
        }

        else if (pathname.endsWith('loans')) {
            setActiveItem('loans')
        }

        else if (pathname.endsWith('favorites')) {
            setActiveItem('favorites')
        }

        else if (pathname.endsWith('notifications')) {
            setActiveItem('notifications')
        }

        else if (pathname.endsWith('profile')){
            setActiveItem('profile')
        }

    }, [pathname])

    const handleViewHomePage = () => {
        router.push('/pages/home')
    }

    const handleViewBooksPage = () => {
        router.push('/pages/books')
    }

    const handleViewEventsPage = () => {
        router.push('/pages/events')
    }

    const handleViewLoansPage = () => {
        router.push('/pages/loans')
    }

    const handleViewFavoritesPage = () => {
        router.push('/pages/favorites')
    }

    const handleViewNotificationsPage = () => {
        router.push('/pages/notifications')
    }

    const handleViewProfilePage = () => {
        router.push('/pages/profile')
    }

    return (
        <div className={navbarStyles.navbar}>
            <div className={navbarStyles.navBarItems}>

                <div className={navbarStyles.navBarTopSection}>
                    <div className={navbarStyles.searchContainer}>

                        <Image
                            className={navbarStyles.schoolAndBookIcon}
                            src={schoolAndBook}
                            alt="school-and-book-icon">
                        </Image>

                        <div className={navbarStyles.searchBar}>
                            <input
                                className={navbarStyles.searchBarInput}
                                placeholder="Pesquisar por livros, eventos, empréstimos realizados, etc...">
                            </input>

                            <div className={navbarStyles.verticalDivisionLine}></div>

                            <div className={navbarStyles.IconWrapper}>
                                <SearchIcon className={navbarStyles.searchBarIcon} />
                            </div>

                        </div>
                    </div>
                </div>

                <div className={navbarStyles.navBarBottomSection}>
                    <div className={navbarStyles.linkList}>

                        <div
                            className={`
                                ${navbarStyles.IconWrapper}
                                ${activeItem === 'home' ? navbarStyles.active : ''}`
                            }
                            onClick={handleViewHomePage}>
                            <HomeIcon className={navbarStyles.linkIcon} sx={{ fontSize: 18 }} />
                            Início
                        </div>

                        <div
                            className={`
                                ${navbarStyles.IconWrapper}
                                ${activeItem === 'books' ? navbarStyles.active : ''}`
                            }
                            onClick={handleViewBooksPage}>
                            <BookIcon className={navbarStyles.linkIcon} sx={{ fontSize: 18 }} />
                            Livros
                        </div>

                        <div className={`
                                ${navbarStyles.IconWrapper}
                                ${activeItem === 'events' ? navbarStyles.active : ''}`
                            }
                            onClick={handleViewEventsPage}>
                            <EventsIcon className={navbarStyles.linkIcon} sx={{ fontSize: 18 }} />
                            Eventos
                        </div>

                        <div className={`
                                ${navbarStyles.IconWrapper}
                                ${activeItem === 'loans' ? navbarStyles.active : ''}`
                            }
                            onClick={handleViewLoansPage}>
                            <LoansIcon className={navbarStyles.linkIcon} sx={{ fontSize: 18 }} />
                            Empréstimos
                        </div>

                        <div className={`
                                ${navbarStyles.IconWrapper}
                                ${activeItem === 'favorites' ? navbarStyles.active : ''}`
                            }
                            onClick={handleViewFavoritesPage}>
                            <FavoritsIcon className={navbarStyles.linkIcon} sx={{ fontSize: 18 }} />
                            Favoritos
                        </div>

                        <div className={`
                                ${navbarStyles.IconWrapper}
                                ${activeItem === 'notifications' ? navbarStyles.active : ''}`
                            }
                            onClick={handleViewNotificationsPage}>
                            <NotificationsIcon className={navbarStyles.linkIcon} sx={{ fontSize: 18 }} />
                            Notificações
                        </div>

                        <div className={`
                                ${navbarStyles.IconWrapper}
                                ${activeItem === 'profile' ? navbarStyles.active : ''}`
                            }
                            onClick={handleViewProfilePage}>
                            <ProfileIcon className={navbarStyles.linkIcon} sx={{ fontSize: 18 }} />
                            Perfil
                        </div>
                    </div>
                </div>

            </div>
        </div>
    )
}
