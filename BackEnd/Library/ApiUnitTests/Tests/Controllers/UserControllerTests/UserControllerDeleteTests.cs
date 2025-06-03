using API.Controllers;
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

namespace ApiUnitTests.Tests.Controllers.UserControllerTests
{
    [Collection("UserControllerCollection")]
    public class UserControllerDeleteTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly UserController _controller;

        public UserControllerDeleteTests(UserControllerFixture fixture)
        {
            _userRepositoryMock = fixture.UserRepositoryMock;
            _controller = fixture.Controller;
        }

        [Fact]
        public async Task DeleteUser_ReturnNoContent()
        {
            var validId = "V4LID-ID";

            _userRepositoryMock.Setup(service => service.DeleteUserAsync(validId))
                .ReturnsAsync(RepositoryStatus.Success);

            var deleteResult = await _controller.Delete(validId);
            Assert.IsType<NoContentResult>(deleteResult);
        }

        [Fact]
        public async Task DeleteUser_ReturnBadRequest_WhenIdIsInvalid()
        {
            var invalidId = "INV4LID-ID";

            _userRepositoryMock.Setup(service => service.DeleteUserAsync(invalidId))
                .ReturnsAsync(RepositoryStatus.InvalidId);

            var deleteResult = await _controller.Delete(invalidId);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(deleteResult);
            var response = Assert.IsType<ApiResponse>(badRequestResult.Value);

            Assert.Equal("Bad Request", response.Status);
            Assert.Null(response.Data);
            Assert.Equal("O id do usuário não pode ser nulo ou conter espaços em brancos", response.Message);
        }

        [Fact]
        public async Task DeletUser_ReturnNotFound_WhenUserNotFound()
        {
            var userId = "US3R-ID";

            _userRepositoryMock.Setup(service => service.DeleteUserAsync(userId))
                .ReturnsAsync(RepositoryStatus.UserNotFound);

            var deleteResult = await _controller.Delete(userId);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(deleteResult);
            var response = Assert.IsType<ApiResponse>(notFoundResult.Value);

            Assert.Equal("Not Found", response.Status);
            Assert.Null(response.Data);
            Assert.Equal($"O usuário de id '{userId}' não foi encontrado", response.Message);
        }

        [Fact]
        public async Task DeletUser_ReturnConflict_WhenFailedToDeleteUser()
        {
            var userId = "US3R-ID";

            _userRepositoryMock.Setup(service => service.DeleteUserAsync(userId))
                .ReturnsAsync(RepositoryStatus.Failed);

            var deleteResult = await _controller.Delete(userId);
            var conflictResult = Assert.IsType<ConflictObjectResult>(deleteResult);
            var response = Assert.IsType<ApiResponse>(conflictResult.Value);

            Assert.Equal("Conflict", response.Status);
            Assert.Null(response.Data);
            Assert.Equal($"Erro inesperado ao tentar deletar o usuário de id '{userId}'", response.Message);
        }
    }
}
