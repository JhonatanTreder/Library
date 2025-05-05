using API.DTO.Book;
using API.DTO.Responses;
using API.DTOs.Book;
using API.Enum;
using API.Enum.Responses;
using API.Models;

namespace API.Repositories.Interfaces
{
    public interface IBookRepository
    {
        Task<RepositoryResponse<BookReturnDTO>> AddBookAsync(CreateBookDTO book);
        Task<RepositoryResponse<IEnumerable<BookCopyReturnDTO>>> AddBookCopiesAsync(CreateBookCopyDTO bookCopyDTO);
        Task<RepositoryStatus> DeleteBookAsync(int id);
        Task<RepositoryStatus> DeleteBookCopyAsync(int bookId, int copyId);
        Task<RepositoryResponse<BookReturnDTO>> GetBookByIdAsync(int id);
        Task<RepositoryResponse<BookCopyReturnDTO>> GetBookCopyByIdAsync(int bookId);
        Task<RepositoryResponse<IEnumerable<BookReturnDTO>>> GetBooksAsync(BookFilterDTO filterBookDTO);
        Task<RepositoryResponse<IEnumerable<BookCopyReturnDTO>>> GetBookCopiesAsync(int bookId);
        Task<RepositoryResponse<IEnumerable<BookReturnDTO>>> GetAvailableBooksAsync();
        Task<RepositoryResponse<IEnumerable<BookCopyReturnDTO>>> GetAvailableBookCopiesAsync(int bookId);
        Task<RepositoryResponse<IEnumerable<BookReturnDTO>>> GetBorrowedBooksAsync();
        Task<RepositoryResponse<IEnumerable<BookCopyReturnDTO>>> GetBorrowedBookCopiesAsync(int bookId);
        Task<RepositoryStatus> UpdateBookAsync(int id, BookUpdateDTO updateBookDTO);
        Task<RepositoryStatus> UpdateBookStatusAsync(int copyId, BookStatus bookStatus);
    }
}
