using API.DTO.Book;
using API.DTO.Responses;
using API.Enum.Responses;
using API.Models;

namespace API.Repositories.Interfaces
{
    public interface IBookRepository
    {
        Task<RepositoryResponse<Book>> AddBookAsync(CreateBookDTO book);
        Task<RepositoryStatus> DeleteBookAsync(int id);
        Task<RepositoryResponse<Book>> GetBookByIdAsync(int id);
        Task<RepositoryResponse<IEnumerable<Book>>> GetBooksAsync(BookFilterDTO filterBookDTO);
        Task<RepositoryResponse<IEnumerable<Book>>> GetAvailableBooksAsync();
        Task<RepositoryResponse<IEnumerable<Book>>> GetBorrowedBooksAsync();
        Task<RepositoryStatus> UpdateBookAsync(int id, BookUpdateDTO updateBookDTO);
    }
}
