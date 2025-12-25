export interface RepositoryResponse<T> {
    status: number;
    data?: T;
    message?: string;
}
