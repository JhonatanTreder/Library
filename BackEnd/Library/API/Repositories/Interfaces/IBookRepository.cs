using API.DTO.Book;
using API.Enum.Responses;
using API.Models;

namespace API.Repositories.Interfaces
{
    public interface IBookRepository
    {
        Task<Book?> AddBookAsync(CreateBookDTO book);
        Task<BookResponse> DeleteBookAsync(int id);
        Task<Book?> GetBookByIdAsync(int id);
        Task<IEnumerable<Book?>> GetBooksAsync(BookFilterDTO filterBookDTO);
        Task<IEnumerable<Book?>> GetAvailableBooksAsync();
        Task<IEnumerable<Book?>> GetBorrowedBooksAsync();
        Task<BookResponse> UpdateBookAsync(int id, BookUpdateDTO updateBookDTO);
    }
}
