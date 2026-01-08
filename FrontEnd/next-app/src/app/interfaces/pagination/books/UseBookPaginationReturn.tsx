import { BookReturnDTO } from '@/app/interfaces/books/BookReturnDTO';
import { SortDirection, SortField, SortState } from '@/app/types/sortTypes';

export interface UseBookPaginationReturn{
    books: BookReturnDTO[];
    loading: boolean;
    error: string | null;
    pagination:{
        pageNumber: number;
        pageSize: number;
        totalItems: number;
        totalPages: number;
        hasPrevious: boolean;
        hasNext: boolean;
    }

    sort: SortState

    fetchBooks: () => Promise<void>;
    handlePageChange: (page: number) => void
    handlePageSizeChange: (size: number) => void
    handleRedirect: () => void
}