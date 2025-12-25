export interface PaginatedDataDTO<T> {
    currentPage: number;
    totalPages: number;
    totalItems: number;
    hasNext: boolean;
    hasPrevious: boolean;
    data: T[];
}