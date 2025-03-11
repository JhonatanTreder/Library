using API.DTO.Book;
using API.Models;

namespace API.Repositories.Interfaces
{
    public interface IBookRepository
    {
        Task<Book> AddBookAsync(Book book);
        Task<IEnumerable<Book>> GetBooksAsync(BookFilterDTO filterBookDTO);
        Task<Book?> GetBookByIdAsync(int id);
        Task<bool> UpdateBookAsync(int id, BookUpdateDTO updateBookDTO);
        Task<bool> DeleteBookAsync(int id);
    }
}
