export type SortField =
| 'title'
| 'author'
| 'publicationYear'
| 'category'
| 'publisher'
| 'availableCopies'
| 'totalCopies'

export type SortDirection = 'asc' | 'desc'

export interface SortOption {
    field: SortField
    label: string
}

export interface SortState {
    field: SortField
    direction: SortDirection
}

export const createSortOptions = <T extends readonly SortOption[]>(options: T): T => options;

export const DEFAULT_SORT_FIELD: SortField = 'title'
export const DEFAULT_SORT_DIRECTION: SortDirection = 'asc'

export const DEFAULT_SORT_OPTIONS: SortOption[] = [
    { field: 'title', label: 'Título' },
    { field: 'author', label: 'Autor' },
    { field: 'publisher', label: 'Editora' },
    { field: 'category', label: 'Categoria' },
    { field: 'totalCopies', label: 'Total de Cópias' },
    { field: 'availableCopies', label: 'Cópias Disponíveis'} ,
    { field: 'publicationYear', label: 'Ano de Publicação' }
] as const;