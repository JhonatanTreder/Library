using API.Controllers;
using API.DTO.Responses;
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
    public class BookControllerDeleteTests
    {
        private readonly BookController _controller;
        private readonly Mock<IBookRepository> _bookRepositoryMock;
        public BookControllerDeleteTests(BookControllerFixture fixture)
        {
            _controller = fixture.Controller;
            _bookRepositoryMock = fixture.BookRepositoryMock;
        }

        [Fact]
        public async Task DeleteBook_ReturnNoContent()
        {
            int validId = 1;

            _bookRepositoryMock.Setup(service => service.DeleteBookAsync(validId))
                .ReturnsAsync(RepositoryStatus.Success);

            var deleteBookResult = await _controller.Delete(validId);
            var noContentResult = Assert.IsType<NoContentResult>(deleteBookResult);
        }

        [Fact]
        public async Task DeleteBook_ReturnNotFound_WhenBookNotFound()
        {
            int invalidId = 1;

            _bookRepositoryMock.Setup(service => service.DeleteBookAsync(invalidId))
                .ReturnsAsync(RepositoryStatus.BookNotFound);

            var deleteBookResult = await _controller.Delete(invalidId);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(deleteBookResult);
            var response = Assert.IsType<ApiResponse>(notFoundResult.Value);

            Assert.Equal("Not Found", response.Status);
            Assert.Null(response.Data);
            Assert.Equal($"O livro de id '{invalidId}' não foi encontrado", response.Message);
        }

        [Fact]
        public async Task DeleteBook_ReturnConflict_WhenCannotDeleteBook()
        {
            int validId = 1;

            _bookRepositoryMock.Setup(service => service.DeleteBookAsync(validId))
                .ReturnsAsync(RepositoryStatus.CannotDelete);

            var deleteBookResult = await _controller.Delete(validId);
            var notFoundResult = Assert.IsType<ConflictObjectResult>(deleteBookResult);
            var response = Assert.IsType<ApiResponse>(notFoundResult.Value);

            Assert.Equal("Conflict", response.Status);
            Assert.Null(response.Data);
            Assert.Equal("Não é possível deletar um livro que o status está em progresso", response.Message);
        }
    }
}
