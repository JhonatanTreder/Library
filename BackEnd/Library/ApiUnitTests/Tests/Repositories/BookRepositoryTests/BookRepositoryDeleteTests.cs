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

        private async Task ClearDatabase()
        {
            _fixture.DbContext.Books.RemoveRange(_fixture.DbContext.Books);
            _fixture.DbContext.Loans.RemoveRange(_fixture.DbContext.Loans);

            await _fixture.DbContext.SaveChangesAsync();
        }
    }
}
