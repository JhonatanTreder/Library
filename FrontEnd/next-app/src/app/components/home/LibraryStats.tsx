"use client"

import libraryStatsStyles from '@/app/components/Styles/home/libraryStats.module.css'

//Icons
import NewBooks from '@mui/icons-material/LibraryAdd'
import BookIcon from '@mui/icons-material/AutoStories'
import UnavailableBooks from '@mui/icons-material/LayersClear'
import BorrowedBooks from '@mui/icons-material/AssignmentReturn'
import EventsIcon from '@mui/icons-material/Event'
import ArrowIcon from '@mui/icons-material/ArrowForward'

import { useEffect, useState } from 'react'
import { useRouter } from 'next/navigation'

interface Book {
    bookId: number;
    title: string;
    author: string;
    description: string;
    publisher: string;
    category: string;
    publicationYear: number;
    totalCopies: number;
    availableCopies: number;
    createdAt: string;
}

interface UnavailableBook {
    copyId: number;
    bookId: number;
    title: string;
    author: string;
    description: string;
    publisher: string;
    category: string;
    publicationYear: number;
    acquiredAt: string;
    status: string;
}

interface Event {
    id: number;
    title: string;
    description: string;
    targetAudience: string;
    location: string;
    startDate: string;
    endDate: string;
    status: number;
}

interface ApiResponse {
    data: {
        recentBooks: Book[];
        totalBooks: Book[];
        delayedBooks: Book[];
        unavailableBooks: UnavailableBook[];
        activeEvents: Event[];
    };
    status: string;
    message: string;
}

interface LibraryStats {
    recentBooksCount: number;
    totalBooksCount: number;
    delayedBooksCount: number;
    unavailableBooksCount: number;
    activeEventsCount: number;
    rawData?: ApiResponse['data'];
}

export default function ShowLibraryStats() {
    const [stats, setStats] = useState<LibraryStats | null>(null)
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState<string | null>(null)
    const router = useRouter()

    const handleViewNewBooks = () => {
        router.push('/pages/books/new')
    }

    const handleViewTotalBooks = () => {
        router.push('/pages/books/')
    }

    const handleViewDelayedBooks = () => {
        router.push('/pages/books/delayed')
    }

    const handleViewUnavailableBooks = () => {
        router.push('/pages/books/unavailable')
    }

    const handleViewActiveEvents = () => {
        router.push('/pages/events/active')
    }

    useEffect(() => {
        fetchDashboardStats()
    }, [])

    async function fetchDashboardStats() {
        try {
            const userToken = localStorage.getItem('token')

            if (!userToken || userToken === undefined) {
                router.push('/auth/login')
            }

            const dashboardRequestURL = `https://localhost:7221/Dashboard`
            const dashboardRequest = await fetch(dashboardRequestURL, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${userToken}`
                }
            })

            if (dashboardRequest.status === 401) {
                localStorage.removeItem('token')
                localStorage.removeItem('refresh-token')
                localStorage.removeItem('toke-expiration-time')
                
                router.push('/auth/login')
                return;
            }

            const dashboardResponse: ApiResponse = await dashboardRequest.json()

            const transformedData: LibraryStats = {
                recentBooksCount: dashboardResponse.data.recentBooks?.length || 0,
                totalBooksCount: dashboardResponse.data.totalBooks?.length || 0,
                delayedBooksCount: dashboardResponse.data.delayedBooks?.length || 0,
                unavailableBooksCount: dashboardResponse.data.unavailableBooks?.length || 0,
                activeEventsCount: dashboardResponse.data.activeEvents?.length || 0,
                rawData: dashboardResponse.data
            }

            setStats(transformedData)
        }
        catch (error) {
            console.error('Erro ao buscar dados:', error)
            setError(error instanceof Error ? error.message : 'Erro desconhecido')
        }
        finally {
            setLoading(false)
        }
    }

    if (loading) {
        return (
            <div className={libraryStatsStyles.libraryContainer}>
                <div style={{ padding: '20px', textAlign: 'center' }}>
                    Carregando estatísticas...
                </div>
            </div>
        )
    }

    if (error) {
        console.log(error)
    }

    if (!stats) {
        return (
            <div className={libraryStatsStyles.libraryContainer}>
                <div style={{ padding: '20px', textAlign: 'center' }}>
                    Não foi possível carregar as estatísticas.
                </div>
            </div>
        )
    }

    return (
        <div className={libraryStatsStyles.libraryContainer}>
            <h1 className={libraryStatsStyles.containerTitle}>
                Estatísticas da Biblioteca
            </h1>

            <section className={libraryStatsStyles.statsSection}>

                <div className={libraryStatsStyles.statsCard}>
                    <NewBooks className={libraryStatsStyles.cardIcon} sx={{ fontSize: 40 }} />
                    <h2 className={libraryStatsStyles.cardTitle}>Novos Livros</h2>
                    <p className={libraryStatsStyles.cardValue}>{stats.recentBooksCount}</p>
                    <button
                        className={`
                            ${libraryStatsStyles.IconWrapper}
                            ${libraryStatsStyles.refButton}`}
                        onClick={handleViewNewBooks}>
                        Exibir Detalhes
                        <ArrowIcon className={libraryStatsStyles.linkIcon} sx={{ fontSize: 18 }} />
                    </button>
                </div>

                <div className={libraryStatsStyles.statsCard}>
                    <BookIcon className={libraryStatsStyles.cardIcon} sx={{ fontSize: 40 }} />
                    <h2 className={libraryStatsStyles.cardTitle}>Total de Livros</h2>
                    <p className={libraryStatsStyles.cardValue}>{stats.totalBooksCount}</p>
                    <button
                        className={`
                            ${libraryStatsStyles.IconWrapper}
                            ${libraryStatsStyles.refButton}`}
                        onClick={handleViewTotalBooks}>
                        Exibir Detalhes
                        <ArrowIcon className={libraryStatsStyles.linkIcon} sx={{ fontSize: 18 }} />
                    </button>
                </div>

                <div className={libraryStatsStyles.statsCard}>
                    <BorrowedBooks className={libraryStatsStyles.cardIcon} sx={{ fontSize: 40 }} />
                    <h2 className={libraryStatsStyles.cardTitle}>Livros Emprestados</h2>
                    <p className={libraryStatsStyles.cardValue}>{stats.delayedBooksCount}</p>
                    <button
                        className={`
                            ${libraryStatsStyles.IconWrapper}
                            ${libraryStatsStyles.refButton}`}
                        onClick={handleViewDelayedBooks}>
                        Exibir Detalhes
                        <ArrowIcon className={libraryStatsStyles.linkIcon} sx={{ fontSize: 18 }} />
                    </button>
                </div>

                <div className={libraryStatsStyles.statsCard}>
                    <UnavailableBooks className={libraryStatsStyles.cardIcon} sx={{ fontSize: 40 }} />
                    <h2 className={libraryStatsStyles.cardTitle}>Livros Indisponíveis</h2>
                    <p className={libraryStatsStyles.cardValue}>{stats.unavailableBooksCount}</p>
                    <button
                        className={`
                            ${libraryStatsStyles.IconWrapper}
                            ${libraryStatsStyles.refButton}`}
                        onClick={handleViewUnavailableBooks}>
                        Exibir Detalhes
                        <ArrowIcon className={libraryStatsStyles.linkIcon} sx={{ fontSize: 18 }} />
                    </button>
                </div>

                <div className={libraryStatsStyles.statsCard}>
                    <EventsIcon className={libraryStatsStyles.cardIcon} sx={{ fontSize: 40 }} />
                    <h2 className={libraryStatsStyles.cardTitle}>Eventos Ativos</h2>
                    <p className={libraryStatsStyles.cardValue}>{stats.activeEventsCount}</p>
                    <button
                        className={`
                            ${libraryStatsStyles.IconWrapper}
                            ${libraryStatsStyles.refButton}`}
                        onClick={handleViewActiveEvents}>
                        Exibir Detalhes
                        <ArrowIcon className={libraryStatsStyles.linkIcon} sx={{ fontSize: 18 }} />
                    </button>
                </div>

            </section>
        </div>
    )
}