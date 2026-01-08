import { BookReturnDTO } from "@/app/interfaces/books/BookReturnDTO";

export interface BookCardProps {
    book?: BookReturnDTO;
    onClick?: () => void
}