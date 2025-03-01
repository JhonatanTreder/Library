using API.Context;
using API.DTO.Book;
using API.Models;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class BookRepository : IBookRepository
    {
        public readonly AppDbContext _context;

        public BookRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Book> AddBook(Book book)
        {
            _context.Add(book);
            await _context.SaveChangesAsync();
            return book;
        }

        public async Task<bool> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);

            if(book is null)
            {
                return false;
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<Book?> GetBookById(int id)
        {
            return await _context.Books.FindAsync(id);
        }

        public async Task<IEnumerable<Book>> GetBooksAsync(FilterBookDTO filterBookDTO)
        {
            var query = _context.Books.AsQueryable();

            if (!string.IsNullOrEmpty(filterBookDTO.Title))
                query = query.Where(t => t.Title == filterBookDTO.Title);

            if (!string.IsNullOrEmpty(filterBookDTO.Author))
                query = query.Where(a => a.Author == filterBookDTO.Author);

            if (!string.IsNullOrEmpty(filterBookDTO.Category))
                query = query.Where(c => c.Category == filterBookDTO.Category);

            if (filterBookDTO.PublicationYear != 0)
                query = query.Where(y => y.PublicationYear == filterBookDTO.PublicationYear);

            if (!string.IsNullOrEmpty(filterBookDTO.Publisher))
                query = query.Where(p => p.Publisher == filterBookDTO.Publisher);

            return await query.ToListAsync();
        }

        public async Task<bool> UpdateBook(int id, UpdateBookDTO updateBookDTO)
        {
            var book = await _context.Books.FindAsync(id);

            if(book is null)
            {
                return false;
            }

            book.Description = updateBookDTO.Description;
            book.Quantity = updateBookDTO.Quantity;

            if (updateBookDTO.Quantity.HasValue)
            {
                // Verifica se a quantidade é válida (se for maior que 0)
                if (updateBookDTO.Quantity.Value < 1)
                {
                    return false; // Se o valor for inválido (menor que 1), você pode retornar falso ou lançar uma exceção
                }

                book.Quantity = updateBookDTO.Quantity.Value; // Atribui o valor de Quantity
            }

            _context.Update(book);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
