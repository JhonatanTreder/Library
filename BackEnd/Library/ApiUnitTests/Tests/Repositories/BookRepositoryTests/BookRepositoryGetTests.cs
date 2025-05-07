using API.DTO.Book;
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

        private async Task ClearDatabase()
        {
            _fixture.DbContext.Books.RemoveRange(_fixture.DbContext.Books);
            _fixture.DbContext.Loans.RemoveRange(_fixture.DbContext.Loans);

            await _fixture.DbContext.SaveChangesAsync();
        }
    }
}
