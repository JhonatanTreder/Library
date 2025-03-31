using API.Context;
using API.DTO.Book;
using API.DTO.Responses;
using API.Enum;
using API.Enum.Responses;
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

        public async Task<RepositoryResponse<Book>> AddBookAsync(CreateBookDTO bookDTO)
        {
            if (bookDTO is null)
            {
                return new RepositoryResponse<Book>(RepositoryStatus.NullObject);
            }

            var book = new Book
            {
                Title = bookDTO.Title,
                Author = bookDTO.Author,
                Description = bookDTO.Description,
                PublicationYear = bookDTO.PublicationYear,
                Publisher = bookDTO.Publisher,
                Category = bookDTO.Category,
                Quantity = bookDTO.Quantity,
                Status = BookStatus.Available
            };

            await _context.AddAsync(book);
            await _context.SaveChangesAsync();

            return new RepositoryResponse<Book>(RepositoryStatus.Success, book);
        }

        public async Task<RepositoryStatus> DeleteBookAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book is null)
                return RepositoryStatus.NotFound;

            var bookInProgress = await _context.Loans
                .AnyAsync(loan => loan.BookId == book.Id && loan.Status == LoanStatus.InProgress);

            if (bookInProgress)
                return RepositoryStatus.CannotDelete;

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return RepositoryStatus.Success;
        }

        public async Task<RepositoryResponse<Book>> GetBookByIdAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book is null)
            {
                return new RepositoryResponse<Book>(RepositoryStatus.NotFound);
            }

            else
            {
                return new RepositoryResponse<Book>(RepositoryStatus.Success, book);
            }
        }

        public async Task<RepositoryResponse<IEnumerable<Book>>> GetBooksAsync(BookFilterDTO bookFilterDTO)
        {
            if (bookFilterDTO is null)
            {
                return new RepositoryResponse<IEnumerable<Book>>(RepositoryStatus.NullObject);
            }

            var query = _context.Books.AsQueryable();

            if (!string.IsNullOrWhiteSpace(bookFilterDTO.Title))
                query = query.Where(t => t.Title.ToLower()
                .Contains(bookFilterDTO.Title.ToLower()));

            if (!string.IsNullOrWhiteSpace(bookFilterDTO.Author))
                query = query.Where(a => a.Author.ToLower()
                .Contains(bookFilterDTO.Author.ToLower()));

            if (!string.IsNullOrWhiteSpace(bookFilterDTO.Category))
                query = query.Where(c => c.Category.ToLower()
                .Contains(bookFilterDTO.Category.ToLower()));

            if (bookFilterDTO.PublicationYear > 0)
                query = query.Where(y => 
                y.PublicationYear == bookFilterDTO.PublicationYear);

            if (!string.IsNullOrWhiteSpace(bookFilterDTO.Publisher))
                query = query.Where(p => p.Publisher.ToLower()
                .Contains(bookFilterDTO.Publisher.ToLower()));

            var books = await query.ToListAsync();

            if (books.Count > 0)
            {
                return new RepositoryResponse<IEnumerable<Book>>(RepositoryStatus.Success, books);
            }

            else
            {
                return new RepositoryResponse<IEnumerable<Book>>(RepositoryStatus.NotFound);
            }
        }
        public async Task<RepositoryResponse<IEnumerable<Book>>> GetAvailableBooksAsync()
        {
            var books = await _context.Books
                .Where(book => book.Status == BookStatus.Available)
                .ToListAsync();

            if (books.Any())
            {
                return new RepositoryResponse<IEnumerable<Book>>(RepositoryStatus.Success, books);
            }

            else
            {
                return new RepositoryResponse<IEnumerable<Book>>(RepositoryStatus.NotFound);
            }
        }

        public async Task<RepositoryResponse<IEnumerable<Book>>> GetBorrowedBooksAsync()
        {
            var books = await _context.Books
                 .Where(book => book.Status == BookStatus.Borrowed)
                 .ToListAsync();

            if (books.Any())
            {
                return new RepositoryResponse<IEnumerable<Book>>(RepositoryStatus.Success, books);
            }

            else
            {
                return new RepositoryResponse<IEnumerable<Book>>(RepositoryStatus.NotFound);
            }
        }

        public async Task<RepositoryStatus> UpdateBookAsync(int id, BookUpdateDTO updateBookDTO)
        {
            if (updateBookDTO is null)
                return RepositoryStatus.NullObject;

            var book = await _context.Books.FindAsync(id);

            if (book is null)
                return RepositoryStatus.NotFound;

            if (!string.IsNullOrWhiteSpace(updateBookDTO.Description))
                book.Description = updateBookDTO.Description;

            if (updateBookDTO.Quantity.HasValue)
            {
                var newQuantity = updateBookDTO.Quantity.Value;

                if (newQuantity < 1)
                {
                    return RepositoryStatus.InvalidQuantity;
                }

                book.Quantity = updateBookDTO.Quantity.Value;
            }

            _context.Update(book);
            await _context.SaveChangesAsync();

            return RepositoryStatus.Success;
        }
    }
}
