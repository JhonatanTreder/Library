using API.DTOs.BookDTOs;
using API.Enum;
using API.Enum.Responses;
using API.Models;
using ApiUnitTests.Fixtures.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiUnitTests.Tests.Repositories.BookRepositoryTests
{
    [Collection("BookRepositoryCollection")]
    public class BookRepositoryPutTests
    {
        private readonly BookRepositoryFixture _fixture;

        public BookRepositoryPutTests(BookRepositoryFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task PutBook_ReturnSuccessOperation()
        {
            await ClearDatabase();

            var book = new Book
            {
                Id = 1,
                Category = "Fantasy",
                Copies = new List<BookCopy>()
                {
                    new BookCopy(),
                    new BookCopy(),
                    new BookCopy()
                }
            };

            var newBookInfo = new BookUpdateDTO
            {
                Quantity = 15,
                Description = "Livro clásico de fantasia",
            };

            await _fixture.DbContext.Books.AddAsync(book);
            await _fixture.DbContext.SaveChangesAsync();

            var putResult = await _fixture.BookRepository.UpdateBookAsync(book.Id, newBookInfo);

            Assert.Equal(RepositoryStatus.Success, putResult);
        }

        [Fact]
        public async Task PutBook_ReturnBookNotFoundOperation_WhenBookNotFound()
        {
            await ClearDatabase();

            int bookId = 1;

            var newBookInfo = new BookUpdateDTO
            {
                Quantity = 15,
                Description = "Livro clásico de fantasia",
            };

            var putResult = await _fixture.BookRepository.UpdateBookAsync(bookId, newBookInfo);

            Assert.Equal(RepositoryStatus.BookNotFound, putResult);
        }

        [Fact]
        public async Task PutBook_ReturnInvalidQuantityOperation_WhenQuantityIsInvalid()
        {
            await ClearDatabase();

            var book = new Book
            {
                Id = 1,
                Description = "Description"
            };

            await _fixture.DbContext.Books.AddAsync(book);
            await _fixture.DbContext.SaveChangesAsync();

            var getResult = await _fixture.BookRepository.GetBookByIdAsync(book.Id);

            Assert.Equal(RepositoryStatus.Success, getResult.Status);

            var newBookInfo = new BookUpdateDTO
            {
                Quantity = 0,
                Description = "Livro clásico de fantasia",
            };

            var putResult = await _fixture.BookRepository.UpdateBookAsync(book.Id, newBookInfo);

            Assert.Equal(RepositoryStatus.InvalidQuantity, putResult);
        }

        [Fact]
        public async Task PutBook_ReturnInvalidCopiesQuantityOperation_WhenQuantityIsInvalid()
        {
            await ClearDatabase();

            var book = new Book
            {
                Id = 1,
                Description = "Description",
                Copies = new List<BookCopy>()
                {
                    new BookCopy { Status = BookStatus.Available },
                    new BookCopy { Status = BookStatus.NotAvailable },
                    new BookCopy { Status = BookStatus.NotAvailable }
                }
            };

            await _fixture.DbContext.Books.AddAsync(book);
            await _fixture.DbContext.SaveChangesAsync();

            var getResult = await _fixture.BookRepository.GetBookByIdAsync(book.Id);

            Assert.Equal(RepositoryStatus.Success, getResult.Status);

            var newBookInfo = new BookUpdateDTO
            {
                Quantity = 1,
                Description = "Livro clásico de fantasia",
            };

            var putResult = await _fixture.BookRepository.UpdateBookAsync(book.Id, newBookInfo);

            Assert.Equal(RepositoryStatus.InvalidCopiesQuantity, putResult);
        }

        [Fact]
        public async Task PutBookStatus_ReturnSuccessOperation()
        {
            await ClearDatabase();

            var book = new Book
            {
                Id = 1,
            };

            await _fixture.DbContext.Books.AddAsync(book);
            await _fixture.DbContext.SaveChangesAsync();

            var newBookCopy = new CreateBookCopyDTO
            {
                BookId = book.Id,
                Quantity = 1
            };

            var addCopyResult = await _fixture.BookRepository.AddBookCopiesAsync(newBookCopy);

            Assert.NotNull(addCopyResult.Data);

            var putResult = await _fixture.BookRepository
                .UpdateBookStatusAsync(addCopyResult.Data.First().CopyId, BookStatus.Borrowed);

            var updatedCopy = await _fixture.DbContext.BookCopies
                .FirstOrDefaultAsync(c => c.Id == addCopyResult.Data.First().CopyId);

            Assert.NotNull(updatedCopy);
            Assert.Equal(BookStatus.Borrowed, updatedCopy.Status);
            Assert.Equal(RepositoryStatus.Success, putResult);
        }

        [Fact]
        public async Task PutBookStatus_ReturnBookCopyNotFoundOperation_WhenBookCopyNotFound()
        {
            await ClearDatabase();

            int invalidId = 2;

            var putResult = await _fixture.BookRepository
                .UpdateBookStatusAsync(invalidId, BookStatus.Borrowed);

            Assert.Equal(RepositoryStatus.BookCopyNotFound, putResult);
        }

        [Fact]
        public async Task PutBookStatus_ReturnNoChange_WhenBookStatusNoChange()
        {
            await ClearDatabase();

            var book = new Book 
            {
                Id = 1,
            };

            await _fixture.DbContext.Books.AddAsync(book);
            await _fixture.DbContext.SaveChangesAsync();

            var newBookCopy = new CreateBookCopyDTO
            {
                BookId = book.Id,
                Quantity = 1
            };

            var addCopyResult = await _fixture.BookRepository.AddBookCopiesAsync(newBookCopy);

            Assert.NotNull(addCopyResult.Data);

            var putResult = await _fixture.BookRepository
                .UpdateBookStatusAsync(addCopyResult.Data.First().CopyId, BookStatus.Available);

            Assert.Equal(RepositoryStatus.NoChange, putResult);
        }

        [Fact]
        public async Task PutBookStatus_ReturnInvalidStatusTransitionOperation()
        {
            await ClearDatabase();

            var book = new Book
            {
                Id = 1
            };

            await _fixture.DbContext.Books.AddAsync(book);
            await _fixture.DbContext.SaveChangesAsync();

            var newBookCopy = new CreateBookCopyDTO
            {
                BookId = book.Id,
                Quantity = 1
            };

            var addCopyResult = await _fixture.BookRepository.AddBookCopiesAsync(newBookCopy);

            Assert.NotNull(addCopyResult.Data);

            var bookCopyId = addCopyResult.Data.First().CopyId;
            var bookStatus = addCopyResult.Data.First().Status;

            var updateBookStatusToBorrowed = await _fixture.BookRepository
                .UpdateBookStatusAsync(bookCopyId, BookStatus.Borrowed);

            Assert.Equal(RepositoryStatus.Success, updateBookStatusToBorrowed);

            var borrowedCopyBook = await _fixture.DbContext.BookCopies
                .FirstOrDefaultAsync(c => c.Id == bookCopyId);

            Assert.NotNull(borrowedCopyBook);
            Assert.Equal(BookStatus.Borrowed, borrowedCopyBook.Status);

            var putResult = await _fixture.BookRepository
                .UpdateBookStatusAsync(bookCopyId, BookStatus.Available);

            Assert.Equal(RepositoryStatus.InvalidStatusTransition, putResult);
        }

        private async Task ClearDatabase()
        {
            _fixture.DbContext.Books.RemoveRange(_fixture.DbContext.Books);

            await _fixture.DbContext.SaveChangesAsync();
        }
    }
}
