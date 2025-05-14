using API.DTO.Book;
using API.DTOs.Book;
using API.Enum;
using API.Enum.Responses;
using API.Models;
using ApiUnitTests.Fixtures.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiUnitTests.Tests.Repositories.BookRepositoryTests
{
    [Collection("BookRepositoryCollection")]
    public class BookRepositoryGetTests
    {
        private readonly BookRepositoryFixture _fixture;

        public BookRepositoryGetTests(BookRepositoryFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task GetBooks_ReturnSuccessOperation()
        {
            await ClearDatabase();

            var book = new Book
            {
                Id = 1,
                Author = "Author",
                Title = "Title"
            };

            var addBook = await _fixture.DbContext.Books.AddAsync(book);
            await _fixture.DbContext.SaveChangesAsync();

            Assert.NotNull(addBook);

            var bookFilter = new BookFilterDTO
            {
                Author = "Author",
                Title = "Title"
            };

            var getResult = await _fixture.BookRepository.GetBooksAsync(bookFilter);

            Assert.Equal(RepositoryStatus.Success, getResult.Status);
        }

        [Fact]
        public async Task GetBooks_ReturnNullObjectOperation_WhenBookFilterIsNull()
        {
            await ClearDatabase();

            var book = new Book
            {
                Id = 1,
                Author = "Author",
                Title = "Title"
            };

            var addBook = await _fixture.DbContext.Books.AddAsync(book);
            await _fixture.DbContext.SaveChangesAsync();

            Assert.NotNull(addBook);

            BookFilterDTO? bookFilter = null;
            var getResult = await _fixture.BookRepository.GetBooksAsync(bookFilter);

            Assert.Equal(RepositoryStatus.NullObject, getResult.Status);
        }

        [Fact]
        public async Task GetBooks_ReturnBookNotFound_WhenBooksNotFound()
        {
            await ClearDatabase();

            var book = new Book
            {
                Id = 1,
            };

            var addBook = await _fixture.DbContext.Books.AddAsync(book);
            await _fixture.DbContext.SaveChangesAsync();

            Assert.NotNull(addBook);

            var bookFilter = new BookFilterDTO
            {
                Author = "Author",
                Title = "Title"
            };

            var getResult = await _fixture.BookRepository.GetBooksAsync(bookFilter);

            Assert.Equal(RepositoryStatus.BookNotFound, getResult.Status);
        }

        [Fact]
        public async Task GetBookCopies_ReturnSuccessOperation()
        {
            await ClearDatabase();

            var book = new Book
            {
                Id = 1,
            };

            var addBook = await _fixture.DbContext.Books.AddAsync(book);
            await _fixture.DbContext.SaveChangesAsync();

            Assert.NotNull(addBook);

            var newCopyInfo = new CreateBookCopyDTO
            {
                BookId = book.Id,
                Quantity = 1
            };

            var addBookCopy = await _fixture.BookRepository.AddBookCopiesAsync(newCopyInfo);

            Assert.NotNull(addBookCopy.Data);
            Assert.Equal(RepositoryStatus.Success, addBookCopy.Status);

            var getResult = await _fixture.BookRepository.GetBookCopiesAsync(addBookCopy.Data.First().BookId);

            Assert.Equal(RepositoryStatus.Success, getResult.Status);
        }

        [Fact]
        public async Task GetBookCopies_ReturnInvalidIdOperation_WhenIdIsInvalid()
        {
            await ClearDatabase();

            var book = new Book
            {
                Id = 1,
            };

            var addBook = await _fixture.DbContext.Books.AddAsync(book);
            await _fixture.DbContext.SaveChangesAsync();

            Assert.NotNull(addBook);

            var newCopyInfo = new CreateBookCopyDTO
            {
                BookId = book.Id,
                Quantity = 1
            };

            var addBookCopy = await _fixture.BookRepository.AddBookCopiesAsync(newCopyInfo);

            Assert.NotNull(addBookCopy.Data);
            Assert.Equal(RepositoryStatus.Success, addBookCopy.Status);

            var getResult = await _fixture.BookRepository.GetBookCopiesAsync(-1);

            Assert.Equal(RepositoryStatus.InvalidId, getResult.Status);
        }

        [Fact]
        public async Task GetBookCopies_ReturnBookNotFoundOperation_WhenBookNotFound()
        {
            await ClearDatabase();

            var book = new Book
            {
                Id = 1,
            };

            var addBook = await _fixture.DbContext.Books.AddAsync(book);
            await _fixture.DbContext.SaveChangesAsync();

            Assert.NotNull(addBook);

            var deleteBook = _fixture.DbContext.Books.Remove(book);

            Assert.NotNull(deleteBook);

            await _fixture.DbContext.SaveChangesAsync();

            var newCopyInfo = new CreateBookCopyDTO
            {
                BookId = book.Id,
                Quantity = 1
            };

            var addBookCopy = await _fixture.BookRepository.AddBookCopiesAsync(newCopyInfo);

            Assert.Null(addBookCopy.Data);

            var getResult = await _fixture.BookRepository.GetBookCopiesAsync(1);

            Assert.Equal(RepositoryStatus.BookNotFound, getResult.Status);
        }

        [Fact]
        public async Task GetBookCopies_ReturnBookCopyNotFoundOperation_WhenCopyNotFound()
        {
            await ClearDatabase();

            var book = new Book
            {
                Id = 1,
            };

            var addBook = await _fixture.DbContext.Books.AddAsync(book);
            await _fixture.DbContext.SaveChangesAsync();

            Assert.NotNull(addBook);

            var newCopyInfo = new CreateBookCopyDTO
            {
                BookId = book.Id,
                Quantity = 1
            };

            var getResult = await _fixture.BookRepository.GetBookCopiesAsync(book.Id);

            Assert.Equal(RepositoryStatus.BookCopyNotFound, getResult.Status);
        }

        [Fact]
        public async Task GetAvailableBooks_ReturnSuccessOperation()
        {
            await ClearDatabase();

            var book = new Book();
            var addBook = await _fixture.DbContext.AddAsync(book);
            await _fixture.DbContext.SaveChangesAsync();

            Assert.NotNull(addBook);


            var bookCopiesInfo = new CreateBookCopyDTO
            {
                BookId = book.Id,
                Quantity = 1
            };

            var addBookCopy = await _fixture.BookRepository.AddBookCopiesAsync(bookCopiesInfo);

            Assert.Equal(RepositoryStatus.Success, addBookCopy.Status);

            var getResult = await _fixture.BookRepository.GetAvailableBooksAsync();

            Assert.Equal(RepositoryStatus.Success, getResult.Status);
        }

        [Fact]
        public async Task GetAvailableBooks_ReturnBookNotFoundOperation_WhenAvailableBooksNotFound()
        {
            await ClearDatabase();

            var book = new Book();
            var addBook = await _fixture.DbContext.AddAsync(book);
            await _fixture.DbContext.SaveChangesAsync();

            Assert.NotNull(addBook);

            var getResult = await _fixture.BookRepository.GetAvailableBooksAsync();

            Assert.Equal(RepositoryStatus.BookNotFound, getResult.Status);
        }

        [Fact]
        public async Task GetAvailableBookCopies_ReturnSuccessOperation()
        {
            await ClearDatabase();

            var book = new Book
            {
                Id = 1
            };

            var addBook = await _fixture.DbContext.Books.AddAsync(book);
            await _fixture.DbContext.SaveChangesAsync();

            Assert.NotNull(addBook);

            var bookCopy = new CreateBookCopyDTO
            {
                BookId = book.Id,
                Quantity = 1
            };

            var addBookCopy = await _fixture.BookRepository.AddBookCopiesAsync(bookCopy);

            Assert.NotNull(addBookCopy);
            Assert.Equal(RepositoryStatus.Success, addBookCopy.Status);

            var getResult = await _fixture.BookRepository.GetAvailableBookCopiesAsync(book.Id);

            Assert.NotNull(getResult);
            Assert.Equal(RepositoryStatus.Success, getResult.Status);
        }

        [Fact]
        public async Task GetAvailableBookCopies_ReturnBookNotFoundOperation_WhenBookNotFound()
        {
            await ClearDatabase();

            var getResult = await _fixture.BookRepository.GetAvailableBookCopiesAsync(1);

            Assert.NotNull(getResult);
            Assert.Equal(RepositoryStatus.BookNotFound, getResult.Status);
        }

        [Fact]
        public async Task GetAvailableBookCopies_ReturnBookCopyNotFound_WhenCopyNotFound()
        {
            await ClearDatabase();

            var book = new Book
            {
                Id = 1
            };

            var addBook = await _fixture.DbContext.Books.AddAsync(book);
            await _fixture.DbContext.SaveChangesAsync();

            Assert.NotNull(addBook);

            var getResult = await _fixture.BookRepository.GetAvailableBookCopiesAsync(book.Id);

            Assert.NotNull(getResult);
            Assert.Equal(RepositoryStatus.BookCopyNotFound, getResult.Status);
        }

        [Fact]
        public async Task GetBorrowedBookCopies_ReturnSuccessOperation()
        {
            await ClearDatabase();

            var book = new Book
            {
                Id = 1
            };

            var addBook = await _fixture.DbContext.Books.AddAsync(book);
            await _fixture.DbContext.SaveChangesAsync();

            Assert.NotNull(addBook);

            var bookCopy = new CreateBookCopyDTO
            {
                BookId = book.Id,
                Quantity = 1
            };

            var addBookCopy = await _fixture.BookRepository.AddBookCopiesAsync(bookCopy);

            Assert.NotNull(addBookCopy.Data);
            Assert.Equal(RepositoryStatus.Success, addBookCopy.Status);

            var updateCopyStatus = await _fixture.BookRepository
                .UpdateBookStatusAsync(addBookCopy.Data.First().CopyId, BookStatus.Borrowed);

            Assert.Equal(RepositoryStatus.Success, updateCopyStatus);

            var getResult = await _fixture.BookRepository.GetBorrowedBookCopiesAsync(book.Id);

            Assert.NotNull(getResult);
            Assert.Equal(RepositoryStatus.Success, getResult.Status);
        }

        [Fact]
        public async Task GetBorrowedBookCopies_ReturnBookNotFoundOperation_WhenBookNotFound()
        {
            await ClearDatabase();

            var getResult = await _fixture.BookRepository.GetBorrowedBookCopiesAsync(1);

            Assert.NotNull(getResult);
            Assert.Equal(RepositoryStatus.BookNotFound, getResult.Status);
        }

        [Fact]
        public async Task GetBorrowedBookCopies_ReturnBookCopyNotFound_WhenCopyNotFound()
        {
            await ClearDatabase();

            var book = new Book
            {
                Id = 1
            };

            var addBook = await _fixture.DbContext.Books.AddAsync(book);
            await _fixture.DbContext.SaveChangesAsync();

            Assert.NotNull(addBook);

            var getResult = await _fixture.BookRepository.GetBorrowedBookCopiesAsync(book.Id);

            Assert.NotNull(getResult);
            Assert.Equal(RepositoryStatus.BookCopyNotFound, getResult.Status);
        }
        private async Task ClearDatabase()
        {
            _fixture.DbContext.Books.RemoveRange(_fixture.DbContext.Books);
            _fixture.DbContext.Loans.RemoveRange(_fixture.DbContext.Loans);

            await _fixture.DbContext.SaveChangesAsync();
        }
    }
}
