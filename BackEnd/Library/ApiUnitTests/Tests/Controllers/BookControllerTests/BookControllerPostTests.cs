using API.Controllers;
using API.DTO.Book;
using API.DTO.Responses;
using API.DTOs.Book;
using API.Enum;
using API.Enum.Responses;
using API.Models;
using API.Repositories.Interfaces;
using ApiUnitTests.Fixtures.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiUnitTests.Tests.Controllers.BookControllerTests
{
    [Collection("BookControllerCollection")]
    public class BookControllerPostTests
    {
        private readonly BookController _controller;
        private readonly Mock<IBookRepository> _bookRepositoryMock;
        public BookControllerPostTests(BookControllerFixture fixture)
        {
            _controller = fixture.Controller;
            _bookRepositoryMock = fixture.BookRepositoryMock;
        }

        [Fact]
        public async Task PostBook_ReturnOk()
        {
            var newBook = new CreateBookDTO
            {
                Title = "valid-title",
                Author = "valid-author",
                Category = "valid-category",
                Description = "valid-description",
                Publisher = "valid-publisher",
                PublicationYear = 2000,
                Quantity = 1,
            };

            var bookInfo = new BookReturnDTO
            {
                Title = newBook.Title,
                Author = newBook.Author,
                Category = newBook.Category,
                Description = newBook.Description,
                Publisher = newBook.Publisher,
                PublicationYear = newBook.PublicationYear,
                TotalCopies = 2,
                AvailableCopies = 1
            };

            _bookRepositoryMock.Setup(service => service.AddBookAsync(newBook))
                .ReturnsAsync(new RepositoryResponse<BookReturnDTO>(RepositoryStatus.Success, bookInfo));

            var postBookResult = await _controller.Post(newBook);
            var okSuccessResult = Assert.IsType<CreatedAtActionResult>(postBookResult);
            var response = Assert.IsType<ApiResponse>(okSuccessResult.Value);

            Assert.Equal("Created", response.Status);
            Assert.NotNull(response.Data);
            Assert.Equal("Livro criado com sucesso", response.Message);
        }

        [Fact]
        public async Task PostBook_ReturnBadRequest_WhenBookIsNull()
        {
            var newBook = new CreateBookDTO
            {
                Title = "",
                Author = "",
                Category = "",
                Description = "",
                Publisher = "",
                PublicationYear = 0,
                Quantity = 0,
            };

            _bookRepositoryMock.Setup(service => service.AddBookAsync(newBook))
                .ReturnsAsync(new RepositoryResponse<BookReturnDTO>(RepositoryStatus.NullObject));

            var postBookResult = await _controller.Post(newBook);
            var okSuccessResult = Assert.IsType<BadRequestObjectResult>(postBookResult);
            var response = Assert.IsType<ApiResponse>(okSuccessResult.Value);

            Assert.Equal("Bad Request", response.Status);
            Assert.Null(response.Data);
            Assert.Equal("O livro não pode ser nulo", response.Message);
        }
    }
}
