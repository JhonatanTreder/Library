export interface BookReturnDTO {
    bookId: number;
    title: string;
    author: string;
    description: string;
    publisher: string;
    publicationYear: number;
    category: string;
    totalCopies: number;
    availableCopies: number;
    createdAt: string;
}