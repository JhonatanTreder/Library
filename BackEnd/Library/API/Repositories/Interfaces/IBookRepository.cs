using API.DTO.Book;
using API.Models;
using Microsoft.EntityFrameworkCore.Update.Internal;

namespace API.Repositories.Interfaces
{
    public interface IBookRepository
    {
        Task<Book> AddBook(Book book);
        Task<IEnumerable<Book>> GetBooksAsync(BookFilterDTO filterBookDTO);
        Task<Book?> GetBookById(int id);
        Task<bool> UpdateBook(int id, UpdateBookDTO updateBookDTO);
        Task<bool> DeleteBook(int id);
    }
}
