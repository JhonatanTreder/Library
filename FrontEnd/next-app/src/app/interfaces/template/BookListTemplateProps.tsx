import { ReactNode } from "react";

export interface BookListTemplateProps {
  // Dados
  books: any[];
  loading: boolean;
  error: string | null;
  
  // Paginação
  pagination?: {
    currentPage: number;
    totalItems: number;
    totalPages: number;
    hasPrevious: boolean;
    hasNext: boolean;
    onPageChange: (page: number) => void;
  };

  ordenation?: {
    sortField: ['title', ]
    sortDirection: ['asc', 'desc']
  }
  
  // Conteúdo customizado
  renderBookCard: (book: any) => ReactNode;
  
  // Estados vazios
  emptyState: {
    message: string;
    subtitle: string;
    actionButton?: ReactNode;
  };
  
  // Opções
  showFilter?: boolean;
}