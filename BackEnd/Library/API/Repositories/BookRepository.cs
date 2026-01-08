using API.Context;
using API.DTOs.BookDTOs;
using API.DTOs.Pagination;
using API.DTOs.Responses;
using API.Enum;
using API.Enum.Responses;
using API.Models;
using API.Pagination;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
                Copies = new List<BookCopy>(),
                CreatedAt = DateTime.UtcNow
            };

            if (bookDTO.Quantity <= 0)
                return new RepositoryResponse<BookReturnDTO>(RepositoryStatus.InvalidQuantity);

            for (int i = 0; i < bookDTO.Quantity; i++)
            {
                book.Copies.Add(new BookCopy
                {
                    Status = BookStatus.Available,
                    Book = book,
                    AcquiredAt = DateTime.UtcNow
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
                AvailableCopies = book.Copies.Count(bc => bc.Status == BookStatus.Available),
                CreatedAt = book.CreatedAt
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
                    AcquiredAt = DateTime.UtcNow,
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
                    Status = copy.Status.ToString(),
                    AcquiredAt = copy.AcquiredAt
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
                AvailableCopies = dbBook.Copies.Count(bc => bc.Status == BookStatus.Available),
                CreatedAt = dbBook.CreatedAt
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
                Status = bookCopy.Status.ToString(),
                AcquiredAt = bookCopy.AcquiredAt
            };

            return new RepositoryResponse<BookCopyReturnDTO>(RepositoryStatus.Success, bookInfo);
        }

        public async Task<RepositoryResponse<PaginatedDataDTO<BookReturnDTO>>> GetBooksWithPaginationAsync(
            PaginationParameters paginationParams,BookFilterDTO? bookFilterDTO = null)
        {
            var query = BuildBooksQuery(bookFilterDTO);

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)paginationParams.PageSize);

            var pagedBooks = await query
                .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
                .Take(paginationParams.PageSize)
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
                    AvailableCopies = b.Copies.Count(copy => copy.Status == BookStatus.Available),
                    CreatedAt = b.CreatedAt
                })
                .ToListAsync();

            if (!pagedBooks.Any())
            {
                return new RepositoryResponse<PaginatedDataDTO<BookReturnDTO>>(RepositoryStatus.BookNotFound);
            }

            var paginatedData = new PaginatedDataDTO<BookReturnDTO>
            {
                CurrentPage = paginationParams.PageNumber,
                TotalPages = totalPages,
                TotalItems = totalItems,
                Data = pagedBooks
            };

            return new RepositoryResponse<PaginatedDataDTO<BookReturnDTO>>(RepositoryStatus.Success, paginatedData);
        }

        public async Task<RepositoryResponse<PaginatedDataDTO<BookReturnDTO>>> GetNewBooksWithPaginationAsync(
            PaginationParameters paginationParams, BookFilterDTO? bookFilterDTO = null, int days = 7)
        {
            var cutOffDate = DateTime.UtcNow.AddDays(-days);

            var query = BuildBooksQuery(bookFilterDTO)
                .Where(b => b.CreatedAt >= cutOffDate);

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)paginationParams.PageSize);

            var pagedBooks = await query
                .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
                .Take(paginationParams.PageSize)
                .Select(b => new BookReturnDTO
                {
                    BookId = b.Id,
                    Title = b.Title,
                    Author = b.Author,
                    Category = b.Category,
                    AvailableCopies = b.Copies.Count(c => c.Status == BookStatus.Available),
                    TotalCopies = b.Copies.Count,
                    Description = b.Description ?? string.Empty,
                    CreatedAt = b.CreatedAt,
                    PublicationYear = b.PublicationYear,
                    Publisher = b.Publisher
                }).ToListAsync();

            if (!pagedBooks.Any())
            {
                return new RepositoryResponse<PaginatedDataDTO<BookReturnDTO>>(RepositoryStatus.BookNotFound);
            }

            var paginatedData = new PaginatedDataDTO<BookReturnDTO>
            {
                Data = pagedBooks,
                CurrentPage = paginationParams.PageNumber,
                TotalItems = totalItems,
                TotalPages = totalPages
            };

            return new RepositoryResponse<PaginatedDataDTO<BookReturnDTO>>(RepositoryStatus.Success, paginatedData);
        }

        public async Task<RepositoryResponse<IEnumerable<BookReturnDTO>>> GetBooksAsync(BookFilterDTO? bookFilterDTO = null)
        {
            if (bookFilterDTO is null)
            {
                var allBooks = await _context.Books
                  .Include(b => b.Copies)
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
                      AvailableCopies = b.Copies.Count(copy => copy.Status == BookStatus.Available),
                      CreatedAt = b.CreatedAt
                  })
                  .ToListAsync();

                return new RepositoryResponse<IEnumerable<BookReturnDTO>>(RepositoryStatus.Success, allBooks);
            }

            var query = BuildBooksQuery(bookFilterDTO);

            var filteredBooks = await query.Select(b => new BookReturnDTO
            {
                BookId = b.Id,
                Title = b.Title,
                Author = b.Author,
                Description = b.Description ?? string.Empty,
                Publisher = b.Publisher,
                PublicationYear = b.PublicationYear,
                Category = b.Category,
                TotalCopies = b.Copies.Count,
                AvailableCopies = b.Copies.Count(copy => copy.Status == BookStatus.Available),
                CreatedAt = b.CreatedAt
            }).ToListAsync();

            if (filteredBooks.Count > 0)
            {
                return new RepositoryResponse<IEnumerable<BookReturnDTO>>(RepositoryStatus.Success, filteredBooks);
            }

            return new RepositoryResponse<IEnumerable<BookReturnDTO>>(RepositoryStatus.BookNotFound);
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
                AcquiredAt = copy.AcquiredAt
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
                    AvailableCopies = b.Copies.Count(copy => copy.Status == BookStatus.Available),
                    CreatedAt = b.CreatedAt
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
            return await GetBookCopiesByStatusAsync(BookStatus.Available, bookId);
        }

        public async Task<RepositoryResponse<IEnumerable<BookReturnDTO>>> GetBorrowedBooksAsync()
        {
            return await GetBookByCopyStatusAsync(BookStatus.Borrowed);
        }

        public async Task<RepositoryResponse<PaginatedDataDTO<BookReturnDTO>>> GetBorrowedBooksWithPaginationAsync(
            PaginationParameters paginationParams, BookFilterDTO bookFilterDTO)
        {
            return await GetBookByCopyStatusWithPaginationAsync(paginationParams, bookFilterDTO, BookStatus.Borrowed);
        }

        public async Task<RepositoryResponse<PaginatedDataDTO<BookReturnDTO>>> GetAvailableBooksWithPaginationAsync(
            PaginationParameters paginationParams, BookFilterDTO bookFilterDTO)
        {
            return await GetBookByCopyStatusWithPaginationAsync(paginationParams, bookFilterDTO, BookStatus.Available);
        }

        public async Task<RepositoryResponse<PaginatedDataDTO<BookReturnDTO>>> GetUnavailableBooksWithPaginationAsync(
            PaginationParameters paginationParams, BookFilterDTO bookFilterDTO)
        {
            var query = BuildBooksQuery(bookFilterDTO);

            query = query.Where(book => !book.Copies.Any(copy => copy.Status == BookStatus.Available));

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)paginationParams.PageSize);

            var pagedBooks = await query
                .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
                .Take(paginationParams.PageSize)
                .Select(b => new BookReturnDTO
                {
                    BookId = b.Id,
                    Author = b.Author,
                    Title = b.Title,
                    Description = b.Description ?? string.Empty,
                    Category = b.Category,
                    AvailableCopies = totalItems,
                    CreatedAt = b.CreatedAt,
                    PublicationYear = b.PublicationYear,
                    Publisher = b.Publisher,
                    TotalCopies = b.Copies.Count
                }).ToListAsync();

            if (!pagedBooks.Any())
            {
                return new RepositoryResponse<PaginatedDataDTO<BookReturnDTO>>(RepositoryStatus.BookNotFound);
            }

            var paginatedData = new PaginatedDataDTO<BookReturnDTO>
            {
                Data = pagedBooks,
                CurrentPage = paginationParams.PageNumber,
                TotalItems = totalItems,
                TotalPages = totalPages
            };

            return new RepositoryResponse<PaginatedDataDTO<BookReturnDTO>>(RepositoryStatus.Success, paginatedData);
        }

        public async Task<RepositoryResponse<IEnumerable<BookCopyReturnDTO>>> GetBorrowedBookCopiesAsync(int bookId)
        {
            return await GetBookCopiesByStatusAsync(BookStatus.Borrowed, bookId);
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
                            Status = BookStatus.Available,
                            AcquiredAt = DateTime.UtcNow
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

        public async Task<RepositoryResponse<IEnumerable<BookReturnDTO>>> GetNewBooksAsync(int days = 7)
        {
            var cutOffDate = DateTime.UtcNow.AddDays(-days);

            var books = await _context.Books
                .Where(b => b.CreatedAt >= cutOffDate)
                .OrderByDescending(b => b.CreatedAt)
                .Select(b => new BookReturnDTO
                {
                    BookId = b.Id,
                    Title = b.Title,
                    Author = b.Author,
                    Category = b.Category,
                    PublicationYear = b.PublicationYear,
                    AvailableCopies = b.Copies.Count(c => c.Status == BookStatus.Available),
                    TotalCopies = b.Copies.Count,
                    Description = b.Description ?? string.Empty,
                    Publisher = b.Publisher,
                    CreatedAt = b.CreatedAt
                })
                .ToListAsync();

            return new RepositoryResponse<IEnumerable<BookReturnDTO>>(RepositoryStatus.Success, books);
        }

        public async Task<RepositoryResponse<IEnumerable<BookCopyReturnDTO>>> GetBookCopiesByStatusAsync(
            BookStatus status, int? bookId = null)
        {
            var query = _context.BookCopies
                .Include(bc => bc.Book)
                .Where(bc => bc.Status == status);

            if (bookId.HasValue && bookId > 0)
            {
                query = query.Where(bc => bc.BookId == bookId.Value);
            }

            var copies = await query
                .Select(bc => new BookCopyReturnDTO
                {
                    CopyId = bc.Id,
                    BookId = bc.BookId,
                    Title = bc.Book.Title,
                    Description = bc.Book.Description ?? "Nenhuma descrição foi fornecida",
                    Author = bc.Book.Author,
                    Category = bc.Book.Category,
                    Publisher = bc.Book.Publisher,
                    PublicationYear = bc.Book.PublicationYear,
                    Status = bc.Status.ToString(),
                    AcquiredAt = bc.AcquiredAt
                })
                .ToListAsync();

            if (copies.Any())
                return new RepositoryResponse<IEnumerable<BookCopyReturnDTO>>(RepositoryStatus.Success, copies);

            return new RepositoryResponse<IEnumerable<BookCopyReturnDTO>>(RepositoryStatus.BookCopyNotFound);
        }

        public async Task<RepositoryResponse<IEnumerable<BookCopyReturnDTO>>> GetBookCopiesByStatusesAsync(IEnumerable<BookStatus> bookStatuses,
            int? bookId = null)
        {
            var query = _context.BookCopies
                .Include(bc => bc.Book)
                .Where(b => bookStatuses.Contains(b.Status));

            if (bookId.HasValue && bookId > 0)
            {
                query = query.Where(bookCopy => bookCopy.BookId == bookId);
            }

            var filteredBookCopies = await query
                .Select(bc => new BookCopyReturnDTO
                {
                    CopyId = bc.Id,
                    BookId = bc.BookId,
                    Title = bc.Book.Title,
                    Author = bc.Book.Author,
                    Category = bc.Book.Category,
                    Description = bc.Book.Description ?? "Nenhuma descrição foi fornecida",
                    Publisher = bc.Book.Publisher,
                    AcquiredAt = bc.AcquiredAt,
                    PublicationYear = bc.Book.PublicationYear,
                    Status = bc.Status.ToString()
                })
                .ToListAsync();


            if (filteredBookCopies.Any())
                return new RepositoryResponse<IEnumerable<BookCopyReturnDTO>>(RepositoryStatus.Success, filteredBookCopies);

            else return new RepositoryResponse<IEnumerable<BookCopyReturnDTO>>(RepositoryStatus.BookCopyNotFound);
        }

        public async Task<RepositoryResponse<PaginatedDataDTO<BookReturnDTO>>> GetBookByCopyStatusWithPaginationAsync(
            PaginationParameters paginationParams, BookFilterDTO bookFilterDTO, BookStatus bookStatus)
        {
            var query = BuildBooksQuery(bookFilterDTO);
            query = query.Where(book => book.Copies.Any(bookCopy => bookCopy.Status == bookStatus));

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)paginationParams.PageSize);

            var pagedBooks = await query
                .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
                .Take(paginationParams.PageSize)
                .Select(b => new BookReturnDTO
                {
                    BookId = b.Id,
                    Title = b.Title,
                    Author = b.Author,
                    Description = b.Description,
                    AvailableCopies = b.Copies.Count(book => book.Status == BookStatus.Available),
                    CreatedAt = b.CreatedAt,
                    Category = b.Category,
                    Publisher = b.Publisher,
                    PublicationYear = b.PublicationYear,
                    TotalCopies = b.Copies.Count
                }).ToListAsync();

            if (!pagedBooks.Any())
            {
                return new RepositoryResponse<PaginatedDataDTO<BookReturnDTO>>(RepositoryStatus.BookNotFound);
            }

            var paginatedData = new PaginatedDataDTO<BookReturnDTO>
            {
                Data = pagedBooks,
                CurrentPage = paginationParams.PageNumber,
                TotalItems = totalItems,
                TotalPages = totalPages
            };

            return new RepositoryResponse<PaginatedDataDTO<BookReturnDTO>>(RepositoryStatus.Success, paginatedData);
        }

        public async Task<RepositoryResponse<IEnumerable<BookReturnDTO>>> GetBookByCopyStatusAsync(BookStatus bookStatus)
        {
            var books = await _context.Books
                .Include(b => b.Copies)
                .Where(book => book.Copies.Any(copy => copy.Status == bookStatus))
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
                    AvailableCopies = b.Copies.Count(copy => copy.Status == BookStatus.Available),
                    CreatedAt = b.CreatedAt
                })
                .ToListAsync();

            if (books.Any())
                return new RepositoryResponse<IEnumerable<BookReturnDTO>>(RepositoryStatus.Success, books);

            else return new RepositoryResponse<IEnumerable<BookReturnDTO>>(RepositoryStatus.BookNotFound);
        }

        public async Task<RepositoryResponse<IEnumerable<BookCopyReturnDTO>>> GetUnavailableBookCopiesAsync(int? bookId = null)
        {
            var unavailableBookCopies = new List<BookStatus>
            {
                BookStatus.NotAvailable,
                BookStatus.Damaged,
                BookStatus.Borrowed,
                BookStatus.Lost,
                BookStatus.UnderMaintenance,
                BookStatus.Reserved,
            };

            return await GetBookCopiesByStatusesAsync(unavailableBookCopies, bookId);
        }

        public async Task<RepositoryResponse<IEnumerable<BookReturnDTO>>> GetBooksWithoutAvailableCopiesAsync()
        {
            var unavailableBooks = await _context.Books
                .Include(b => b.Copies)
                .Where(book => !book.Copies.Any(copy => copy.Status == BookStatus.Available))
                .Select(book => new BookReturnDTO
                {
                    BookId = book.Id,
                    Title = book.Title,
                    Author = book.Author,
                    Category = book.Category,
                    Description = book.Description ?? string.Empty,
                    AvailableCopies = 0,
                    TotalCopies = book.Copies.Count,
                    CreatedAt = book.CreatedAt,
                    PublicationYear = book.PublicationYear,
                    Publisher = book.Publisher
                })
                .ToListAsync();

            if (unavailableBooks.Any())
                return new RepositoryResponse<IEnumerable<BookReturnDTO>>(RepositoryStatus.Success, unavailableBooks);

            else
                return new RepositoryResponse<IEnumerable<BookReturnDTO>>(RepositoryStatus.BookNotFound);
        }

        private IQueryable<Book> BuildBooksQuery(BookFilterDTO? bookFilterDTO)
        {
            var query = _context.Books
                .Include(b => b.Copies)
                .AsQueryable();

            if (bookFilterDTO == null)
                return query;

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

            return query;
        }
    }
}
