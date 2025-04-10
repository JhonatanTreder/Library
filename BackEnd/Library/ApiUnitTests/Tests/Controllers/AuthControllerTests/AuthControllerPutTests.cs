using API.Controllers;
using API.DTO.Responses;
using API.Enum.Responses;
using API.Services.Interfaces;
using ApiUnitTests.Fixtures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ApiUnitTests.Tests.Controllers.AuthControllerTests
{
    [Collection("AuthServiceCollection")]
    public class AuthControllerPutTests
    {
        private readonly AuthController _controller;
        private readonly Mock<IAuthService> _authServiceMock;

        public AuthControllerPutTests(AuthServiceFixture fixture)
        {
            _controller = fixture.AuthController;
            _authServiceMock = fixture.MockAuthService;
        }

        [Fact]
        public async Task RevokeToken_ReturnNoContent()
        {
            string username = "valid-username";

            _authServiceMock.Setup(service => service.RevokeToken(username))
                .ReturnsAsync(RepositoryStatus.Success);

            var revokeTokenResult = await _controller.RevokeToken(username);
            var noContentResult = Assert.IsType<NoContentResult>(revokeTokenResult);

            Assert.Equal(StatusCodes.Status204NoContent, noContentResult.StatusCode);
        }

        [Fact]
        public async Task RevokeToken_ReturnBadRequest_WhenUsernameIsNullOrWhiteSpace()
        {
            string username = "";

            _authServiceMock.Setup(service => service.RevokeToken(username))
                .ReturnsAsync(RepositoryStatus.NullObject);

            var revokeTokenResult = await _controller.RevokeToken(username);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(revokeTokenResult);

            var response = Assert.IsType<ApiResponse>(badRequestResult.Value);

            Assert.Equal("Bad Request", response.Status);
            Assert.Null(response.Data);
            Assert.Equal("O nome do usuário não pode ser nulo ou conter espaços vazios", response.Message);
        }

        [Fact]
        public async Task RevokeToken_ReturnNotFound_WhenUserNotFound()
        {
            string username = "user";

            _authServiceMock.Setup(service => service.RevokeToken(username))
                .ReturnsAsync(RepositoryStatus.UserNotFound);

            var revokeTokenResult = await _controller.RevokeToken(username);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(revokeTokenResult);

            var response = Assert.IsType<ApiResponse>(notFoundResult.Value);

            Assert.Equal("Not Found", response.Status);
            Assert.Null(response.Data);
            Assert.Equal($"O usuário '{username}' não foi encontrado", response.Message);
        }

        [Fact]
        public async Task RevokeToken_ReturnConflict_WhenFailedToRevokeToken()
        {
            string username = "user";

            _authServiceMock.Setup(service => service.RevokeToken(username))
                .ReturnsAsync(RepositoryStatus.FailedToRevokeToken);

            var revokeTokenResult = await _controller.RevokeToken(username);
            var conflictResult = Assert.IsType<ConflictObjectResult>(revokeTokenResult);

            var response = Assert.IsType<ApiResponse>(conflictResult.Value);

            Assert.Equal("Conflict", response.Status);
            Assert.Null(response.Data);
            Assert.Equal($"Falha ao revogar o token para o usuário '{username}'", response.Message);
        }
    }
}
