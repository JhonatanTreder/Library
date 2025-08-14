using API.Controllers;
using API.DTOs.BookDTOs;
using API.DTOs.Responses;
using API.Enum.Responses;
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
    public class BookControllerPutTests
    {
        private readonly BookController _controller;
        private readonly Mock<IBookRepository> _bookRepositoryMock;

        public BookControllerPutTests(BookControllerFixture fixture)
        {
            _controller = fixture.Controller;
            _bookRepositoryMock = fixture.BookRepositoryMock;
        }

        [Fact]
        public async Task PutBook_ReturnNoContent()
        {
            var validId = 1;
            var bookUpdate = new BookUpdateDTO
            {
                Description = "valid-description",
                Quantity = 2
            };

            _bookRepositoryMock.Setup(service => service.UpdateBookAsync(validId, bookUpdate))
                .ReturnsAsync(RepositoryStatus.Success);

            var updateBookResult = await _controller.Put(validId, bookUpdate);
            var noContentResult = Assert.IsType<NoContentResult>(updateBookResult);
        }

        [Fact]
        public async Task PutBook_ReturnBadRequest_WhenBookIsNull()
        {
            var bookId = 1;
            var bookUpdate = new BookUpdateDTO();

            _bookRepositoryMock.Setup(service => service.UpdateBookAsync(bookId, bookUpdate))
                .ReturnsAsync(RepositoryStatus.NullObject);

            var updateBookResult = await _controller.Put(bookId, bookUpdate);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(updateBookResult);
            var response = Assert.IsType<ApiResponse>(badRequestResult.Value);

            Assert.Equal("Bad Request", response.Status);
            Assert.Null(response.Data);
            Assert.Equal("O livro não pode ser nulo", response.Message);
        }

        [Fact]
        public async Task PutBook_ReturnBadRequest_WhenBookQuantityIsInvalid()
        {
            var bookId = 1;
            var bookUpdate = new BookUpdateDTO 
            {
                Quantity = 0
            };

            _bookRepositoryMock.Setup(service => service.UpdateBookAsync(bookId, bookUpdate))
                .ReturnsAsync(RepositoryStatus.InvalidQuantity);

            var updateBookResult = await _controller.Put(bookId, bookUpdate);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(updateBookResult);
            var response = Assert.IsType<ApiResponse>(badRequestResult.Value);

            Assert.Equal("Bad Request", response.Status);
            Assert.Null(response.Data);
            Assert.Equal("A nova quantidade de livros deve ser maior que zero", response.Message);
        }

        [Fact]
        public async Task PutBook_ReturnConflict_WhenBookCopiesQuantityIsInvalid()
        {
            var bookId = 1;
            var newBookInfo = new BookUpdateDTO
            {
                Quantity = 1
            };

            _bookRepositoryMock.Setup(service => service.UpdateBookAsync(bookId, newBookInfo))
                .ReturnsAsync(RepositoryStatus.InvalidCopiesQuantity);

            var putResult = await _controller.Put(bookId, newBookInfo);
            var conflictResult = Assert.IsType<ConflictObjectResult>(putResult);
            var response = Assert.IsType<ApiResponse>(conflictResult.Value);

            Assert.Equal("Conflict", response.Status);
            Assert.Null(response.Data);
            Assert.Equal("Não é possível atualizar a quantidade de livros quando o número de cópias disponíveis são inválidas", response.Message);
        }

        [Fact]
        public async Task PutBook_ReturnNotFound_WhenBookNotFound()
        {
            var bookId = 1;
            var bookUpdate = new BookUpdateDTO
            {
                Description = "description",
                Quantity = 2
            };

            _bookRepositoryMock.Setup(service => service.UpdateBookAsync(bookId, bookUpdate))
                .ReturnsAsync(RepositoryStatus.BookNotFound);

            var updateBookResult = await _controller.Put(bookId, bookUpdate);
            var badRequestResult = Assert.IsType<NotFoundObjectResult>(updateBookResult);
            var response = Assert.IsType<ApiResponse>(badRequestResult.Value);

            Assert.Equal("Not Found", response.Status);
            Assert.Null(response.Data);
            Assert.Equal($"O livro de id '{bookId}' não foi encontrado", response.Message);
        }
    }
}
