using API.Controllers;
using API.DTO.Book;
using API.DTO.Responses;
using API.DTOs.Book;
using API.Enum.Responses;
using API.Models;
using API.Repositories.Interfaces;
using ApiUnitTests.Fixtures;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ApiUnitTests.Tests.Controllers.BookControllerTests
{
    [Collection("BookControllerCollection")]
    public class BookControllerGetTests
    {
        private readonly BookController _controller;
        private readonly Mock<IBookRepository> _bookRepositoryMock;
        public BookControllerGetTests(BookControllerFixture fixture)
        {
            _controller = fixture.Controller;
            _bookRepositoryMock = fixture.BookRepositoryMock;
        }

        [Fact]
        public async Task GetBooks_ReturnOk()
        {
            var bookFilter = new BookFilterDTO
            {
                Title = "valid-title",
                Author = "valid-auhtor",
                Category = "valid-category",
                Publisher = "valid-publisher",
                PublicationYear = 2000,
            };

            var booksQuery = new List<BookReturnDTO>
            {
                new BookReturnDTO(),
                new BookReturnDTO(),
                new BookReturnDTO()
            };

            _bookRepositoryMock.Setup(service => service.GetBooksAsync(bookFilter))
                .ReturnsAsync(new RepositoryResponse<IEnumerable<BookReturnDTO>>(RepositoryStatus.Success, booksQuery));

            var okSuccessResult = await _controller.Get(bookFilter);
            var getBooksResult = Assert.IsType<OkObjectResult>(okSuccessResult);
            var response = Assert.IsType<ApiResponse>(getBooksResult.Value);

            Assert.Equal("Ok", response.Status);
            Assert.NotNull(response.Data);
            Assert.Equal("Livros encontrados com sucesso", response.Message);
        }

        [Fact]
        public async Task GetBooks_ReturnBadRequest_WhenBookFilterIsNull()
        {
            var bookFilter = new BookFilterDTO
            {
                Title = "",
                Author = "",
                Category = "",
                Publisher = "",
                PublicationYear = null,
            };

            _bookRepositoryMock.Setup(service => service.GetBooksAsync(bookFilter))
                .ReturnsAsync(new RepositoryResponse<IEnumerable<BookReturnDTO>>(RepositoryStatus.NullObject));

            var okSuccessResult = await _controller.Get(bookFilter);
            var getBooksResult = Assert.IsType<BadRequestObjectResult>(okSuccessResult);
            var response = Assert.IsType<ApiResponse>(getBooksResult.Value);

            Assert.Equal("Bad Request", response.Status);
            Assert.Null(response.Data);
            Assert.Equal("O livro não pode ser nulo", response.Message);
        }
    }
}
