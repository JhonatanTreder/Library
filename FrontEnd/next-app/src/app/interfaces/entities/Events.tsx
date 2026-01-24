// Interfaces para Dashboard

export interface DashboardEvent {
    id: string
    title: string
    description: string
    targetAudience: string
    location: string
    startDate: Date
    endDate: Date
    eventStatus: string
    cancelledAt: Date | null
    cancellationReason: string | null
    isArchived: boolean
}

export interface FavoriteBook {
    id: string
    userId: string
    bookId: string
    favoritedAt: Date
    book: {
        id: string
        title: string
        author: string
    }
}

export interface UserDashboardData {
    totalLoans: number
    eventsHeld: DashboardEvent[]
    favoriteBooks: FavoriteBook[]
    totalFines: number
    entryDate: Date | null
}

export interface DashboardDisplayData {
    totalLoans: string
    totalEvents: string
    favoriteBooksCount: string
    totalFines: string
    entryDate: string
}