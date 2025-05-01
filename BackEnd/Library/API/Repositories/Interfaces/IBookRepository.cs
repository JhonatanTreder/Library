using API.DTO.Book;
using API.DTO.Responses;
using API.DTOs.Book;
using API.Enum.Responses;
using API.Models;

namespace API.Repositories.Interfaces
{
    public interface IBookRepository
    {
        Task<RepositoryResponse<BookReturnDTO>> AddBookAsync(CreateBookDTO book);
        Task<RepositoryResponse<BookReturnDTO>> AddBookCopyAsync(int bookId);
        Task<RepositoryStatus> DeleteBookAsync(int id);
        Task<RepositoryStatus> DeleteBookCopyAsync(int bookId, int copyId);
        Task<RepositoryResponse<BookReturnDTO>> GetBookByIdAsync(int id);
        Task<RepositoryResponse<BookReturnDTO>> GetBookCopyByIdAsync(int bookId);
        Task<RepositoryResponse<IEnumerable<BookReturnDTO>>> GetBooksAsync(BookFilterDTO filterBookDTO);
        Task<RepositoryResponse<IEnumerable<BookReturnDTO>>> GetAvailableBooksAsync();
        Task<RepositoryResponse<IEnumerable<BookReturnDTO>>> GetBorrowedBooksAsync();
        Task<RepositoryStatus> UpdateBookAsync(int id, BookUpdateDTO updateBookDTO);
    }
}
