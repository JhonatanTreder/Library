import { SortDirection, SortField } from "@/app/types/sortTypes";
import { ReactNode } from "react";

export interface BookListTemplateProps {
  
  books: any[];
  loading: boolean;
  error: string | null;

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

  renderBookCard: (book: any) => ReactNode;

  emptyState: {
    message: string;
    subtitle: string;
    actionButton?: ReactNode;
  };

  showFilter?: boolean;
}