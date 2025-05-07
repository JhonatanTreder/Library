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
    public class BookRepositoryDeleteTests
    {
        private readonly BookRepositoryFixture _fixture;

        public BookRepositoryDeleteTests(BookRepositoryFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task DeleteBook_ReturnSuccessOperation()
        {
            await ClearDatabase();

            var book = new Book
            {
                Id = 1,
                Author = "Valid Author",
            };

            await _fixture.DbContext.Books.AddAsync(book);
            await _fixture.DbContext.SaveChangesAsync();

            var deleteResult = await _fixture.BookRepository.DeleteBookAsync(book.Id);
            var repositoryResponse = await _fixture.BookRepository.GetBookByIdAsync(book.Id);

            Assert.Equal(RepositoryStatus.Success, deleteResult);
            Assert.Equal(RepositoryStatus.BookNotFound, repositoryResponse.Status);
        }

        [Fact]
        public async Task DeleteBook_ReturnNotFoundOperation_WhenBookNotFoundInDatabase()
        {
            await ClearDatabase();

            var deleteResult = await _fixture.BookRepository.DeleteBookAsync(1);

            Assert.Equal(RepositoryStatus.NotFound, deleteResult);
        }

        [Fact]
        public async Task DeleteBook_ReturnCannotDeleteOperation_WhenDateIsInvalid()
        {
            await ClearDatabase();

            var book = new Book
            {
                Id = 1,
                Author = "Book Author",
            };

            var loan = new Loan
            {
                Id = 1,
                BookId = book.Id,
                Status = LoanStatus.InProgress
            };

            await _fixture.DbContext.Books.AddAsync(book);
            await _fixture.DbContext.Loans.AddAsync(loan);
            await _fixture.DbContext.SaveChangesAsync();

            var deleteResult = await _fixture.BookRepository.DeleteBookAsync(book.Id);

            Assert.Equal(RepositoryStatus.CannotDelete, deleteResult);
        }

        [Fact]
        public async Task DeleteBookCopy_ReturnSuccessOperation()
        {
            await ClearDatabase();

            var book = new Book
            {
                Id = 1,
            };

            var addBook = await _fixture.DbContext.AddAsync(book);
            await _fixture.DbContext.SaveChangesAsync();

            Assert.NotNull(addBook);

            var newBookCopyInfo = new CreateBookCopyDTO
            {
                BookId = book.Id,
                Quantity = 2
            };

            var addBookCopy = await _fixture.BookRepository
                .AddBookCopiesAsync(newBookCopyInfo);

            Assert.NotNull(addBookCopy.Data);

            var deleteBookCopy = await _fixture.BookRepository
                .DeleteBookCopyAsync(book.Id, addBookCopy.Data.First().CopyId);

            Assert.Equal(RepositoryStatus.Success, deleteBookCopy);
        }

        [Fact]
        public async Task DeleteBookCopy_ReturnInvalidIdOperation_WhenIdIsInvalid()
        {
            await ClearDatabase();

            var book = new Book
            {
                Id = 1,
            };

            var addBook = await _fixture.DbContext.AddAsync(book);
            await _fixture.DbContext.SaveChangesAsync();

            Assert.NotNull(addBook);

            var newBookCopyInfo = new CreateBookCopyDTO
            {
                BookId = -1,
                Quantity = 2
            };

            var deleteBookCopy = await _fixture.BookRepository
               .DeleteBookCopyAsync(book.Id, newBookCopyInfo.BookId);

            Assert.Equal(RepositoryStatus.InvalidId, deleteBookCopy);
        }

        [Fact]
        public async Task DeleetBookCopy_ReturnBookNotFoundOperation_WhenBookNotFound()
        {
            await ClearDatabase();

            var book = new Book
            {
                Id = 1,
            };

            var addBook = await _fixture.DbContext.AddAsync(book);
            await _fixture.DbContext.SaveChangesAsync();

            Assert.NotNull(addBook);

            var newBookCopyInfo = new CreateBookCopyDTO
            {
                BookId = book.Id,
                Quantity = 2
            };

            var addBookCopy = await _fixture.BookRepository
                .AddBookCopiesAsync(newBookCopyInfo);

            Assert.NotNull(addBookCopy.Data);

            var deleteBook = await _fixture.BookRepository.DeleteBookAsync(book.Id);

            Assert.Equal(RepositoryStatus.Success, deleteBook);

            var deleteBookCopy = await _fixture.BookRepository
                .DeleteBookCopyAsync(book.Id, addBookCopy.Data.First().CopyId);

            Assert.Equal(RepositoryStatus.BookNotFound, deleteBookCopy);
        }

        [Fact]
        public async Task DeleteBookCopy_ReturnBookCopyNotFoundOperation_WhenBookCopyNotFound()
        {
            await ClearDatabase();

            var book = new Book
            {
                Id = 1,
            };

            var addBook = await _fixture.DbContext.AddAsync(book);
            await _fixture.DbContext.SaveChangesAsync();

            Assert.NotNull(addBook);

            var newBookCopyInfo = new CreateBookCopyDTO
            {
                BookId = book.Id,
                Quantity = 2
            };

            var deleteBookCopy = await _fixture.BookRepository
                .DeleteBookCopyAsync(book.Id, 2);

            Assert.Equal(RepositoryStatus.BookCopyNotFound, deleteBookCopy);
        }

        [Fact]
        public async Task DeleteBookCopy_ReturnBookCopyDoesNotBelongToBookOperation()
        {
            await ClearDatabase();

            var book = new Book 
            {
                Id = 1 
            };

            var otherBook = new Book 
            { 
                Id = 2
            };

            await _fixture.DbContext.AddAsync(book);
            await _fixture.DbContext.AddAsync(otherBook);
            await _fixture.DbContext.SaveChangesAsync();

            var newBookCopyInfo = new CreateBookCopyDTO
            {
                BookId = book.Id,
                Quantity = 1
            };

            var addBookCopy = await _fixture.BookRepository
                .AddBookCopiesAsync(newBookCopyInfo);

            Assert.NotNull(addBookCopy.Data);

            var bookCopy = addBookCopy.Data.First();
            var deleteResult = await _fixture.BookRepository
                .DeleteBookCopyAsync(otherBook.Id, bookCopy.CopyId);

            Assert.Equal(RepositoryStatus.BookCopyDoesNotBelongToBook, deleteResult);
        }


        private async Task ClearDatabase()
        {
            _fixture.DbContext.Books.RemoveRange(_fixture.DbContext.Books);
            _fixture.DbContext.Loans.RemoveRange(_fixture.DbContext.Loans);

            await _fixture.DbContext.SaveChangesAsync();
        }
    }
}
