using API.DTOs.BookDTOs;
using API.DTOs.Responses;
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
        Task<RepositoryResponse<IEnumerable<BookReturnDTO>>> GetBooksAsync(BookFilterDTO? filterBookDTO = null);
        Task<RepositoryResponse<IEnumerable<BookReturnDTO>>> GetRecentBooksAsync(int days = 7);
        Task<RepositoryResponse<IEnumerable<BookCopyReturnDTO>>> GetBookCopiesAsync(int bookId);
        Task<RepositoryResponse<IEnumerable<BookReturnDTO>>> GetAvailableBooksAsync();
        Task<RepositoryResponse<IEnumerable<BookCopyReturnDTO>>> GetAvailableBookCopiesAsync(int bookId);
        Task<RepositoryResponse<IEnumerable<BookReturnDTO>>> GetBorrowedBooksAsync();
        Task<RepositoryResponse<IEnumerable<BookCopyReturnDTO>>> GetBorrowedBookCopiesAsync(int bookId);
        Task<RepositoryResponse<IEnumerable<BookCopyReturnDTO>>> GetBookCopiesByStatusAsync(
            BookStatus status, int? bookId = null);
        Task<RepositoryResponse<IEnumerable<BookCopyReturnDTO>>> GetBookCopiesByStatusesAsync(IEnumerable<BookStatus> bookStatuses,
            int? bookId = null);
        Task<RepositoryResponse<IEnumerable<BookReturnDTO>>> GetBookByCopyStatusAsync(BookStatus bookStatus);
        Task<RepositoryResponse<IEnumerable<BookCopyReturnDTO>>> GetUnavailableBookCopiesAsync(int? bookId = null);
        Task<RepositoryResponse<IEnumerable<BookReturnDTO>>> GetBooksWithoutAvailableCopiesAsync();
        Task<RepositoryStatus> UpdateBookAsync(int id, BookUpdateDTO updateBookDTO);
        Task<RepositoryStatus> UpdateBookStatusAsync(int copyId, BookStatus bookStatus);
    }
}
