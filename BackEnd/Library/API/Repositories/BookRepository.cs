using API.Context;
using API.DTO.Book;
using API.DTO.Responses;
using API.DTOs.Book;
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

        public async Task<RepositoryResponse<BookReturnDTO>> AddBookAsync(CreateBookDTO? bookDTO)
        {
            if (bookDTO is null)
                return new RepositoryResponse<BookReturnDTO>(RepositoryStatus.NullObject);

            var book = new Book
            {
                Title = bookDTO.Title,
                Author = bookDTO.Author,
                Description = bookDTO.Description,
                PublicationYear = bookDTO.PublicationYear,
                Publisher = bookDTO.Publisher,
                Category = bookDTO.Category,
                Copies = new List<BookCopy>()
            };

            if (bookDTO.Quantity <= 0)
                return new RepositoryResponse<BookReturnDTO>(RepositoryStatus.InvalidQuantity);

            for (int i = 0; i < bookDTO.Quantity; i++)
            {
                book.Copies.Add(new BookCopy
                {
                    Status = BookStatus.Available,
                    Book = book
                });
            }

            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();

            var bookReturn = new BookReturnDTO
            {
                BookId = book.Id,
                Title = book.Title,
                Author = book.Author,
                Description = book.Description ?? string.Empty,
                PublicationYear = book.PublicationYear,
                Publisher = book.Publisher,
                Category = book.Category,
                TotalCopies = book.Copies.Count,
                AvailableCopies = book.Copies.Count(bc => bc.Status == BookStatus.Available)
            };

            return new RepositoryResponse<BookReturnDTO>(RepositoryStatus.Success, bookReturn);
        }

        public async Task<RepositoryResponse<IEnumerable<BookCopyReturnDTO>>> AddBookCopiesAsync(CreateBookCopyDTO? bookCopyDTO)
        {
            if (bookCopyDTO is null)
                return new RepositoryResponse<IEnumerable<BookCopyReturnDTO>>(RepositoryStatus.NullObject);

            if (bookCopyDTO.BookId <= 0)
                return new RepositoryResponse<IEnumerable<BookCopyReturnDTO>>(RepositoryStatus.InvalidId);

            var principalBook = await _context.Books
                .Include(b => b.Copies)
                .FirstOrDefaultAsync(b => b.Id == bookCopyDTO.BookId);

            if (principalBook is null)
                return new RepositoryResponse<IEnumerable<BookCopyReturnDTO>>(RepositoryStatus.BookNotFound);

            var bookCopies = new List<BookCopy>();
            var copiesInfo = new List<BookCopyReturnDTO>();

            for (int i = 0; i < bookCopyDTO.Quantity; i++)
            {

                var newBookCopy = new BookCopy
                {
                    BookId = principalBook.Id,
                    Book = principalBook,
                    Status = BookStatus.Available,
                };

                bookCopies.Add(newBookCopy);
            }

            await _context.BookCopies.AddRangeAsync(bookCopies);
            await _context.SaveChangesAsync();

            foreach (var copy in bookCopies)
            {
                copiesInfo.Add(new BookCopyReturnDTO
                {
                    CopyId = copy.Id,
                    BookId = principalBook.Id,
                    Title = principalBook.Title,
                    Description = principalBook.Description ?? "Nenhuma descrição foi fornecida",
                    Author = principalBook.Author,
                    Category = principalBook.Category,
                    Publisher = principalBook.Publisher,
                    PublicationYear = principalBook.PublicationYear,
                    Status = copy.Status.ToString()
                });
            }

            return new RepositoryResponse<IEnumerable<BookCopyReturnDTO>>(RepositoryStatus.Success, copiesInfo);
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

        public async Task<RepositoryStatus> DeleteBookCopyAsync(int bookId, int copyId)
        {
            if (bookId <= 0 || copyId <= 0)
                return RepositoryStatus.InvalidId;

            var book = await _context.Books.FindAsync(bookId);
            var bookCopy = await _context.BookCopies.FindAsync(copyId);

            if (book is null)
                return RepositoryStatus.BookNotFound;

            if (bookCopy is null)
                return RepositoryStatus.BookCopyNotFound;

            if (bookCopy.BookId != bookId)
                return RepositoryStatus.BookCopyDoesNotBelongToBook;

            _context.BookCopies.Remove(bookCopy);
            await _context.SaveChangesAsync();

            return RepositoryStatus.Success;
        }

        public async Task<RepositoryResponse<BookReturnDTO>> GetBookByIdAsync(int id)
        {
            if (id <= 0)
                return new RepositoryResponse<BookReturnDTO>(RepositoryStatus.InvalidId);

            var dbBook = await _context.Books
                .Include(b => b.Copies)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (dbBook is null)
                return new RepositoryResponse<BookReturnDTO>(RepositoryStatus.BookNotFound);

            var bookDTO = new BookReturnDTO
            {
                BookId = dbBook.Id,
                Title = dbBook.Title,
                Author = dbBook.Author,
                Description = dbBook.Description ?? string.Empty,
                Category = dbBook.Category,
                Publisher = dbBook.Publisher,
                PublicationYear = dbBook.PublicationYear,
                TotalCopies = dbBook.Copies.Count,
                AvailableCopies = dbBook.Copies.Count(bc => bc.Status == BookStatus.Available)
            };

            return new RepositoryResponse<BookReturnDTO>(RepositoryStatus.Success, bookDTO);
        }

        public async Task<RepositoryResponse<BookCopyReturnDTO>> GetBookCopyByIdAsync(int copyId)
        {
            if (copyId <= 0)
                return new RepositoryResponse<BookCopyReturnDTO>(RepositoryStatus.InvalidId);

            var bookCopy = await _context.BookCopies
                .Include(bc => bc.Book)
                .FirstOrDefaultAsync(bc => bc.Id == copyId);

            if (bookCopy is null || bookCopy.Book is null)
                return new RepositoryResponse<BookCopyReturnDTO>(RepositoryStatus.BookCopyNotFound);

            var bookInfo = new BookCopyReturnDTO
            {
                CopyId = copyId,
                BookId = bookCopy.Book.Id,
                Title = bookCopy.Book.Title,
                Description = bookCopy.Book.Description ?? "Nenhuma descrição foi fornecida",
                Author = bookCopy.Book.Author,
                Category = bookCopy.Book.Category,
                Publisher = bookCopy.Book.Publisher,
                PublicationYear = bookCopy.Book.PublicationYear,
                Status = bookCopy.Status.ToString()
            };

            return new RepositoryResponse<BookCopyReturnDTO>(RepositoryStatus.Success, bookInfo);
        }

        public async Task<RepositoryResponse<IEnumerable<BookReturnDTO>>> GetBooksAsync(BookFilterDTO bookFilterDTO)
        {
            if (bookFilterDTO is null)
            {
                return new RepositoryResponse<IEnumerable<BookReturnDTO>>(RepositoryStatus.NullObject);
            }

            var query = _context.Books
                .Include(b => b.Copies)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(bookFilterDTO.Title))
                query = query.Where(t => t.Title.ToLower().Contains(bookFilterDTO.Title.ToLower()));

            if (!string.IsNullOrWhiteSpace(bookFilterDTO.Author))
                query = query.Where(a => a.Author.ToLower().Contains(bookFilterDTO.Author.ToLower()));

            if (!string.IsNullOrWhiteSpace(bookFilterDTO.Category))
                query = query.Where(c => c.Category.ToLower().Contains(bookFilterDTO.Category.ToLower()));

            if (bookFilterDTO.PublicationYear > 0)
                query = query.Where(y => y.PublicationYear == bookFilterDTO.PublicationYear);

            if (!string.IsNullOrWhiteSpace(bookFilterDTO.Publisher))
                query = query.Where(p => p.Publisher.ToLower().Contains(bookFilterDTO.Publisher.ToLower()));

            var books = await query.Select(b => new BookReturnDTO
            {
                BookId = b.Id,
                Title = b.Title,
                Author = b.Author,
                Description = b.Description ?? string.Empty,
                Publisher = b.Publisher,
                PublicationYear = b.PublicationYear,
                Category = b.Category,
                TotalCopies = b.Copies.Count,
                AvailableCopies = b.Copies.Count(copy => copy.Status == BookStatus.Available)
            }).ToListAsync();

            if (books.Count > 0)
            {
                return new RepositoryResponse<IEnumerable<BookReturnDTO>>(RepositoryStatus.Success, books);
            }

            return new RepositoryResponse<IEnumerable<BookReturnDTO>>(RepositoryStatus.NotFound);
        }

        public async Task<RepositoryResponse<IEnumerable<BookCopyReturnDTO>>> GetBookCopiesAsync(int bookId)
        {
            if (bookId <= 0)
                return new RepositoryResponse<IEnumerable<BookCopyReturnDTO>>(RepositoryStatus.InvalidId);

            var book = await _context.Books
                .Include(b => b.Copies)
                .FirstOrDefaultAsync(b => b.Id == bookId);

            if (book is null)
                return new RepositoryResponse<IEnumerable<BookCopyReturnDTO>>(RepositoryStatus.BookNotFound);

            if (book.Copies == null || book.Copies.Count == 0)
                return new RepositoryResponse<IEnumerable<BookCopyReturnDTO>>(RepositoryStatus.BookCopyNotFound);

            var bookCopies = book.Copies.Select(copy => new BookCopyReturnDTO
            {
                CopyId = copy.Id,
                BookId = book.Id,
                Title = book.Title,
                Description = book.Description ?? "Nenhuma descrição foi fornecida",
                Author = book.Author,
                Category = book.Category,
                Publisher = book.Publisher,
                PublicationYear = book.PublicationYear,
                Status = copy.Status.ToString(),
            }).ToList();

            return new RepositoryResponse<IEnumerable<BookCopyReturnDTO>>(RepositoryStatus.Success, bookCopies);
        }

        public async Task<RepositoryResponse<IEnumerable<BookReturnDTO>>> GetAvailableBooksAsync()
        {
            var books = await _context.Books
                .Include(b => b.Copies)
                .Where(book => book.Copies.Any(copy => copy.Status == BookStatus.Available))
                .Select(b => new BookReturnDTO
                {
                    BookId = b.Id,
                    Title = b.Title,
                    Author = b.Author,
                    Description = b.Description ?? string.Empty,
                    Publisher = b.Publisher,
                    PublicationYear = b.PublicationYear,
                    Category = b.Category,
                    TotalCopies = b.Copies.Count,
                    AvailableCopies = b.Copies.Count(copy => copy.Status == BookStatus.Available)
                })
                .ToListAsync();

            if (books.Any())
            {
                return new RepositoryResponse<IEnumerable<BookReturnDTO>>(RepositoryStatus.Success, books);
            }

            return new RepositoryResponse<IEnumerable<BookReturnDTO>>(RepositoryStatus.BookNotFound);
        }

        public async Task<RepositoryResponse<IEnumerable<BookCopyReturnDTO>>> GetAvailableBookCopiesAsync(int bookId)
        {
            if (bookId <= 0)
                return new RepositoryResponse<IEnumerable<BookCopyReturnDTO>>(RepositoryStatus.InvalidId);

            var book = await _context.Books
                 .Include(b => b.Copies)
                 .FirstOrDefaultAsync(b => b.Id == bookId);

            if (book is null)
                return new RepositoryResponse<IEnumerable<BookCopyReturnDTO>>(RepositoryStatus.BookNotFound);

            var availableCopies = book.Copies
                .Where(c => c.Status == BookStatus.Available)
                .ToList();

            if (availableCopies == null || availableCopies.Count == 0)
                return new RepositoryResponse<IEnumerable<BookCopyReturnDTO>>(RepositoryStatus.BookCopyNotFound);

            var bookCopies = availableCopies.Select(copy => new BookCopyReturnDTO
            {
                CopyId = copy.Id,
                BookId = book.Id,
                Title = book.Title,
                Description = book.Description ?? "Nenhuma descrição foi fornecida",
                Author = book.Author,
                Category = book.Category,
                Publisher = book.Publisher,
                PublicationYear = book.PublicationYear,
                Status = BookStatus.Available.ToString()
            }).ToList();

            return new RepositoryResponse<IEnumerable<BookCopyReturnDTO>>(RepositoryStatus.Success, bookCopies);
        }

        public async Task<RepositoryResponse<IEnumerable<BookReturnDTO>>> GetBorrowedBooksAsync()
        {
            var books = await _context.Books
                .Include(b => b.Copies)
                .Where(book => book.Copies.Any(copy => copy.Status == BookStatus.Borrowed))
                .Select(b => new BookReturnDTO
                {
                    BookId = b.Id,
                    Title = b.Title,
                    Author = b.Author,
                    Description = b.Description ?? string.Empty,
                    Publisher = b.Publisher,
                    PublicationYear = b.PublicationYear,
                    Category = b.Category,
                    TotalCopies = b.Copies.Count,
                    AvailableCopies = b.Copies.Count(c => c.Status == BookStatus.Available)
                })
                .ToListAsync();

            if (books.Any())
            {
                return new RepositoryResponse<IEnumerable<BookReturnDTO>>(RepositoryStatus.Success, books);
            }

            return new RepositoryResponse<IEnumerable<BookReturnDTO>>(RepositoryStatus.BookNotFound);
        }

        public async Task<RepositoryResponse<IEnumerable<BookCopyReturnDTO>>> GetBorrowedBookCopiesAsync(int bookId)
        {
            if (bookId <= 0)
                return new RepositoryResponse<IEnumerable<BookCopyReturnDTO>>(RepositoryStatus.InvalidId);

            var book = await _context.Books
                 .Include(b => b.Copies)
                 .FirstOrDefaultAsync(b => b.Id == bookId);

            if (book is null)
                return new RepositoryResponse<IEnumerable<BookCopyReturnDTO>>(RepositoryStatus.BookNotFound);

            var borrowedCopies = book.Copies
                .Where(c => c.Status == BookStatus.Borrowed)
                .ToList();

            if (borrowedCopies == null || borrowedCopies.Count == 0)
                return new RepositoryResponse<IEnumerable<BookCopyReturnDTO>>(RepositoryStatus.BookCopyNotFound);

            var bookCopies = borrowedCopies.Select(copy => new BookCopyReturnDTO
            {
                CopyId = copy.Id,
                BookId = book.Id,
                Title = book.Title,
                Description = book.Description ?? "Nenhuma descrição foi fornecida",
                Author = book.Author,
                Category = book.Category,
                Publisher = book.Publisher,
                PublicationYear = book.PublicationYear,
                Status = BookStatus.Borrowed.ToString()
            }).ToList();

            return new RepositoryResponse<IEnumerable<BookCopyReturnDTO>>(RepositoryStatus.Success, bookCopies);
        }

        public async Task<RepositoryStatus> UpdateBookAsync(int id, BookUpdateDTO updateBookDTO)
        {
            if (updateBookDTO is null)
                return RepositoryStatus.NullObject;

            var book = await _context.Books
                .Include(b => b.Copies)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book is null)
                return RepositoryStatus.BookNotFound;

            if (!string.IsNullOrWhiteSpace(updateBookDTO.Description))
                book.Description = updateBookDTO.Description;

            if (updateBookDTO.Quantity.HasValue)
            {
                var currentCount = book.Copies.Count;
                var newQuantity = updateBookDTO.Quantity.Value;

                if (newQuantity < 1)
                    return RepositoryStatus.InvalidQuantity;

                if (newQuantity > currentCount)
                {
                    var copiesToAdd = newQuantity - currentCount;

                    for (int i = 0; i < copiesToAdd; i++)
                    {
                        book.Copies.Add(new BookCopy
                        {
                            BookId = book.Id,
                            Status = BookStatus.Available
                        });
                    }
                }

                else if (newQuantity < currentCount)
                {
                    var availableCopies = book.Copies
                        .Where(c => c.Status == BookStatus.Available)
                        .Take(currentCount - newQuantity)
                        .ToList();

                    if (availableCopies.Count < (currentCount - newQuantity))
                        return RepositoryStatus.InvalidCopiesQuantity;

                    _context.BookCopies.RemoveRange(availableCopies);
                }
            }

            _context.Books.Update(book);
            await _context.SaveChangesAsync();

            return RepositoryStatus.Success;
        }

        public async Task<RepositoryStatus> UpdateBookStatusAsync(int copyId, BookStatus bookStatus)
        {
            var bookCopy = await _context.BookCopies.FindAsync(copyId);

            if (bookCopy is null)
                return RepositoryStatus.BookCopyNotFound;

            if (bookCopy.Status == bookStatus)
                return RepositoryStatus.NoChange;

            if (bookCopy.Status == BookStatus.Borrowed && bookStatus == BookStatus.Available)
                return RepositoryStatus.InvalidStatusTransition;

            bookCopy.Status = bookStatus;

            _context.BookCopies.Update(bookCopy);
            await _context.SaveChangesAsync();

            return RepositoryStatus.Success;
        }
    }
}
