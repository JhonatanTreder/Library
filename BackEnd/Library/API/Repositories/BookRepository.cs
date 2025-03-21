using API.Context;
using API.DTO.Book;
using API.DTO.Responses;
using API.Models;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly AppDbContext _context;

        public BookRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Book?> AddBookAsync(CreateBookDTO bookDTO)
        {
            if (bookDTO is null)
            {
                return null;
            }

            var book = new Book
            {
                Title = bookDTO.Title,
                Author = bookDTO.Author,
                Description = bookDTO.Description,
                PublicationYear = bookDTO.PublicationYear,
                Publisher = bookDTO.Publisher,
                Category = bookDTO.Category,
                Quantity = bookDTO.Quantity
            };

            await _context.AddAsync(book);

            await _context.SaveChangesAsync();

            return book;
        }

        public async Task<bool> DeleteBookAsync(int id)
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

        public async Task<Book?> GetBookByIdAsync(int id)
        {
            return await _context.Books.FindAsync(id);
        }

        public async Task<IEnumerable<Book?>> GetBooksAsync(BookFilterDTO filterBookDTO)
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

        public async Task<bool> UpdateBookAsync(int id, BookUpdateDTO updateBookDTO)
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
                if (updateBookDTO.Quantity.Value < 1)
                {
                    return false;
                }

                book.Quantity = updateBookDTO.Quantity.Value;
            }

            _context.Update(book);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
