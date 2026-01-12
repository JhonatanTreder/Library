import { SortDirection, SortField } from "@/app/types/sortTypes";
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

    ordernation?: {
        sortField: SortField;
        sortDirection: SortDirection;
        handleSortFieldChange: (element: React.ChangeEvent<HTMLSelectElement>) => void;
        handleSortDirectionChange: (element: React.ChangeEvent<HTMLSelectElement>) => void;
    }
}