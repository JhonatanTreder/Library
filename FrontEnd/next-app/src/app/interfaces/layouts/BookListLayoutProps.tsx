import { ReactNode } from "react"

export interface BookListLayoutProps {
    children: ReactNode;
    showFilter?: boolean;
    showPaginationTop?: boolean;
    showPaginationBottom?: boolean;
    pagination?: {
        currentPage: number;
        totalItems: number;
        totalPages: number;
        hasPrevious: boolean;
        hasNext: boolean;
        onPageChange: (page: number) => void;
    };
}