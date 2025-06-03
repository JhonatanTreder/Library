using API.DTOs.Book;
using API.DTOs.Book;
using API.Enum.Responses;
using API.Models;
using ApiUnitTests.Fixtures.Repositories;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiUnitTests.Tests.Repositories.BookRepositoryTests
{
    [Collection("BookRepositoryCollection")]
    public class BookRepositoryPostTests
    {
        private readonly BookRepositoryFixture _fixture;

        public BookRepositoryPostTests(BookRepositoryFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task PostBook_ReturnSuccessOperation()
        {
            await ClearDatabase();

            var newBook = new CreateBookDTO
            {
                Title = "Harry Potter"
            };

            var postResult = await _fixture.BookRepository.AddBookAsync(newBook);

            Assert.Equal(RepositoryStatus.Success, postResult.Status);
        }

        [Fact]
        public async Task PostBook_ReturnNullObject_WhenBookInfoIsNul()
        {
            await ClearDatabase();

            CreateBookDTO? newBook = null;

            var postResult = await _fixture.BookRepository.AddBookAsync(newBook);

            Assert.Equal(RepositoryStatus.NullObject, postResult.Status);
        }

        [Fact]
        public async Task PostBook_ReturnInvalidQuantityOperation_WhenQuantityIsInvalid()
        {
            await ClearDatabase();

            var newBook = new CreateBookDTO
            {
                Title = "Harry Potter",
                Quantity = -1
            };

            var postResult = await _fixture.BookRepository.AddBookAsync(newBook);

            Assert.Equal(RepositoryStatus.InvalidQuantity, postResult.Status);
        }

        [Fact]
        public async Task PostBookCopy_ReturnSuccessOperation()
        {
            await ClearDatabase();
            
            var book = new Book
            {
                Id = 1,
            };

            await _fixture.DbContext.Books.AddAsync(book);
            await _fixture.DbContext.SaveChangesAsync();

            var newBooks = new CreateBookCopyDTO
            {
                BookId = book.Id,
                Quantity = 2
            };

            var postResult = await _fixture.BookRepository.AddBookCopiesAsync(newBooks);

            Assert.NotNull(postResult.Data);
            Assert.Equal(RepositoryStatus.Success, postResult.Status);
        }

        [Fact]
        public async Task PostBookCopy_ReturnNullObjectOperation_WhenNewCopyInfoIsNull()
        {
            await ClearDatabase();

            var book = new Book
            {
                Id = 1,
            };

            await _fixture.DbContext.Books.AddAsync(book);
            await _fixture.DbContext.SaveChangesAsync();

            CreateBookCopyDTO? newBooks = null;

            var postResult = await _fixture.BookRepository.AddBookCopiesAsync(newBooks);

            Assert.Equal(RepositoryStatus.NullObject, postResult.Status);
        }

        [Fact]
        public async Task PostBookCopy_ReturnBookNotFoundOperation_WhenBookNotFoundInDatabase()
        {
            await ClearDatabase();

            var newBooks = new CreateBookCopyDTO
            {
                BookId = 1,
                Quantity = 2
            };

            var postResult = await _fixture.BookRepository.AddBookCopiesAsync(newBooks);

            Assert.Equal(RepositoryStatus.BookNotFound, postResult.Status);
        }

        [Fact]
        public async Task PostBookCopy_ReturnInvalidId_WhenBookIdIsInvalid()
        {
            await ClearDatabase();

            var newBooks = new CreateBookCopyDTO
            {
                BookId = -1,
                Quantity = 2
            };

            var postResult = await _fixture.BookRepository.AddBookCopiesAsync(newBooks);

            Assert.Equal(RepositoryStatus.InvalidId, postResult.Status);
        }

        private async Task ClearDatabase()
        {
            _fixture.DbContext.Books.RemoveRange(_fixture.DbContext.Books);

            await _fixture.DbContext.SaveChangesAsync();
        }
    }
}
