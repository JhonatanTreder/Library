using API.Controllers;
using API.DTOs.Authentication;
using API.DTOs.Responses;
using API.DTOs.Token;
using API.DTOs.User;
using API.Enum;
using API.Enum.Responses;
using API.Services.Interfaces;
using ApiUnitTests.Fixtures.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ApiUnitTests.Tests.Controllers.AuthControllerTests
{
    [Collection("AuthServiceCollection")]
    public class AuthControllerPostTests
    {
        private readonly AuthController _controller;
        private readonly Mock<IAuthService> _authServiceMock;

        public AuthControllerPostTests(AuthServiceFixture fixture)
        {
            _controller = fixture.AuthController;
            _authServiceMock = fixture.MockAuthService;
        }

        [Fact]
        public async Task LoginUser_ReturnBadRequest_WhenUserIsNull()
        {
            var user = new LoginDTO
            {
                Email = "test",
                Password = "test",
            };

            _authServiceMock.Setup(service => service.Login(user))
                .ReturnsAsync(new RepositoryResponse<TokenReturnDTO>(RepositoryStatus.NullObject));

            var loginResult = await _controller.Login(user);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(loginResult);
            var response = Assert.IsType<ApiResponse>(badRequestResult.Value);

            Assert.Equal("Bad Request", response.Status);
            Assert.Null(response.Data);
            Assert.Equal("A requisição de login não pode ser nula", response.Message);
        }

        [Fact]
        public async Task LoginUser_ReturnOk()
        {
            var user = new LoginDTO
            {
                Email = "valid@email",
                Password = "validPassword"
            };

            var tokens = new TokenReturnDTO
            {
                Token = "valid-token",
                RefreshToken = "valid-refresh-token",
                Expiration = DateTime.UtcNow
            };

            _authServiceMock.Setup(service => service.Login(user))
                .ReturnsAsync(new RepositoryResponse<TokenReturnDTO>(RepositoryStatus.Success, tokens));

            var loginResult = await _controller.Login(user);
            var okSuccessResult = Assert.IsType<OkObjectResult>(loginResult);
            var response = Assert.IsType<ApiResponse>(okSuccessResult.Value);

            Assert.Equal("Ok", response.Status);
            Assert.NotNull(response.Data);
            Assert.Equal("Usuário logado com sucesso", response.Message);
        }

        [Fact]
        public async Task LoginUser_ReturnUnauthorized_WhenUserIsUnauthorized()
        {
            var user = new LoginDTO
            {
                Email = "email",
                Password = "password"
            };

            _authServiceMock.Setup(service => service.Login(user))
                .ReturnsAsync(new RepositoryResponse<TokenReturnDTO>(RepositoryStatus.Unauthorized));

            var loginResult = await _controller.Login(user);
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(loginResult);
            var response = Assert.IsType<ApiResponse>(unauthorizedResult.Value);

            Assert.Equal("Unauthorized", response.Status);
            Assert.Null(response.Data);
            Assert.Equal("Credenciais inválidas", response.Message);
        }

        [Fact]
        public async Task LoginUser_Return500_WhenFailedToUpdateUser()
        {
            var user = new LoginDTO
            {
                Email = "email",
                Password = "password"
            };

            _authServiceMock.Setup(service => service.Login(user))
                .ReturnsAsync(new RepositoryResponse<TokenReturnDTO>(RepositoryStatus.FailedToUpdateUser));

            var loginResult = await _controller.Login(user);
            var objectResult = Assert.IsType<ObjectResult>(loginResult);

            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);

            var response = Assert.IsType<ApiResponse>(objectResult.Value);

            Assert.Equal("Internal Server Error", response.Status);
            Assert.Null(response.Data);
            Assert.Equal("Erro inesperado ao atualizar o usuário com as novas informações do Refresh Token", response.Message);
        }

        [Fact]
        public async Task RegisterUser_ReturnOk()
        {
            var registerUser = new RegisterDTO()
            {
                Email = "test",
                Name = "test",
                Password = "test"
            };

            var registeredUser = new UserDTO
            {
                Email = "test",
                Name = "test",
                UserType = UserType.User.ToString()
            };

            _authServiceMock.Setup(service => service.Register(registerUser))
                .ReturnsAsync(new RepositoryResponse<UserDTO>(RepositoryStatus.Success, registeredUser));

            var registerResult = await _controller.Register(registerUser);
            var okSuccessResult = Assert.IsType<OkObjectResult>(registerResult);

            var response = Assert.IsType<ApiResponse>(okSuccessResult.Value);
            var userResponse = Assert.IsType<UserDTO>(response.Data);

            Assert.Equal("Ok", response.Status);
            Assert.NotNull(response.Data);
            Assert.Equal("Usuário registrado com sucesso", response.Message);
            Assert.Equal(registeredUser.UserType.ToString(), userResponse.UserType.ToString());
        }

        [Fact]
        public async Task RegisterUser_ReturnBadRequest_WhenUserIsNull()
        {
            var registerUser = new RegisterDTO
            {
                Name = "test",
                Email = "test",
                Password = "test"
            };

            _authServiceMock.Setup(service => service.Register(registerUser))
                .ReturnsAsync(new RepositoryResponse<UserDTO>(RepositoryStatus.NullObject));

            var registerResult = await _controller.Register(registerUser);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(registerResult);
            var response = Assert.IsType<ApiResponse>(badRequestResult.Value);

            Assert.Equal("Bad Request", response.Status);
            Assert.Null(response.Data);
            Assert.Equal("A requisição de registro não pode ser nula", response.Message);
        }

        [Fact]
        public async Task RegisterUser_ReturnBadRequest_WhenPasswordLengthIsInvalid()
        {
            var registerUser = new RegisterDTO
            {
                Name = "test",
                Email = "test",
                Password = "test"
            };

            _authServiceMock.Setup(service => service.Register(registerUser))
                .ReturnsAsync(new RepositoryResponse<UserDTO>(RepositoryStatus.InvalidPassword));

            var registerResult = await _controller.Register(registerUser);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(registerResult);
            var response = Assert.IsType<ApiResponse>(badRequestResult.Value);

            Assert.Equal("Bad Request", response.Status);
            Assert.Null(response.Data);
            Assert.Equal("A senha deve ter no mínimo 6 caracteres", response.Message);
        }

        [Fact]
        public async Task RegisterUser_ReturnConflict_WhenEmailAlreadyExists()
        {
            var registerUser = new RegisterDTO
            {
                Name = "test",
                Email = "test",
                Password = "test"
            };

            _authServiceMock.Setup(service => service.Register(registerUser))
                .ReturnsAsync(new RepositoryResponse<UserDTO>(RepositoryStatus.EmailAlreadyExists));

            var registerResult = await _controller.Register(registerUser);
            var badRequestResult = Assert.IsType<ConflictObjectResult>(registerResult);
            var response = Assert.IsType<ApiResponse>(badRequestResult.Value);

            Assert.Equal("Conflict", response.Status);
            Assert.Null(response.Data);
            Assert.Equal("O Email especificado já está sendo utilizado", response.Message);
        }

        [Fact]
        public async Task RegisterUser_Return500_WhenFailedToCreateUser()
        {
            var registerUser = new RegisterDTO
            {
                Name = "test",
                Email = "test",
                Password = "test"
            };

            _authServiceMock.Setup(service => service.Register(registerUser))
                .ReturnsAsync(new RepositoryResponse<UserDTO>(RepositoryStatus.FailedToCreateUser));

            var registerResult = await _controller.Register(registerUser);
            var objectResult = Assert.IsType<ObjectResult>(registerResult);

            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);

            var response = Assert.IsType<ApiResponse>(objectResult.Value);

            Assert.Equal("Internal Server Error", response.Status);
            Assert.Null(response.Data);
            Assert.Equal("Erro inesperado ao tentar criar um usuário", response.Message);
        }

        [Fact]
        public async Task RegisterUser_Return500_WhenFailedToCreateRole()
        {
            var registerUser = new RegisterDTO
            {
                Name = "test",
                Email = "test",
                Password = "test"
            };

            _authServiceMock.Setup(service => service.Register(registerUser))
                .ReturnsAsync(new RepositoryResponse<UserDTO>(RepositoryStatus.FailedToCreateRole));

            var registerResult = await _controller.Register(registerUser);
            var objectResult = Assert.IsType<ObjectResult>(registerResult);

            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);

            var response = Assert.IsType<ApiResponse>(objectResult.Value);

            Assert.Equal("Internal Server Error", response.Status);
            Assert.Null(response.Data);
            Assert.Equal("Erro inesperado ao tentar criar a role para o usuário", response.Message);
        }

        [Fact]
        public async Task RegisterUser_Return500_WhenInvalidRoleAssignment()
        {
            var registerUser = new RegisterDTO
            {
                Name = "test",
                Email = "test",
                Password = "test"
            };

            _authServiceMock.Setup(service => service.Register(registerUser))
                .ReturnsAsync(new RepositoryResponse<UserDTO>(RepositoryStatus.InvalidRoleAssignment));

            var registerResult = await _controller.Register(registerUser);
            var objectResult = Assert.IsType<ObjectResult>(registerResult);

            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);

            var response = Assert.IsType<ApiResponse>(objectResult.Value);

            Assert.Equal("Internal Server Error", response.Status);
            Assert.Null(response.Data);
            Assert.Equal("Erro inesperado ao tentar atribuir uma role para o usuário", response.Message);
        }

        [Fact]
        public async Task RefreshToken_ReturnOk()
        {
            var tokens = new TokenDTO
            {
                AccessToken = "valid-access-token",
                RefreshToken = "valid-refresh-token"
            };

            var newTokens = new TokenDTO
            {
                AccessToken = "new-access-token",
                RefreshToken = "new-refresh-token"
            };

            _authServiceMock.Setup(service => service.RefreshToken(tokens))
                .ReturnsAsync(new RepositoryResponse<TokenDTO>(RepositoryStatus.Success, newTokens));

            var refreshTokenResult = await _controller.RefreshToken(tokens);
            var okSuccessResult = Assert.IsType<OkObjectResult>(refreshTokenResult);
            var response = Assert.IsType<ApiResponse>(okSuccessResult.Value);

            Assert.Equal("Ok", response.Status);
            Assert.NotNull(response.Data);
            Assert.Equal("Refresh Token criado com sucesso", response.Message);
        }

        [Fact]
        public async Task RefreshToken_ReturnBadRequest_WhenTokenIsNull()
        {
            var tokens = new TokenDTO
            {
                AccessToken = "",
                RefreshToken = ""
            };

            _authServiceMock.Setup(service => service.RefreshToken(tokens))
                .ReturnsAsync(new RepositoryResponse<TokenDTO>(RepositoryStatus.NullObject));

            var refreshTokenResult = await _controller.RefreshToken(tokens);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(refreshTokenResult);
            var response = Assert.IsType<ApiResponse>(badRequestResult.Value);

            Assert.Equal("Bad Request", response.Status);
            Assert.Null(response.Data);
            Assert.Equal("O token não pode ser nulo", response.Message);
        }

        [Fact]
        public async Task RefreshToken_ReturnUnauthorized_WhenInvalidRefreshToken()
        {
            var tokens = new TokenDTO
            {
                AccessToken = "test",
                RefreshToken = "test"
            };

            _authServiceMock.Setup(service => service.RefreshToken(tokens))
                .ReturnsAsync(new RepositoryResponse<TokenDTO>(RepositoryStatus.InvalidRefreshToken));

            var refreshTokenResult = await _controller.RefreshToken(tokens);
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(refreshTokenResult);
            var response = Assert.IsType<ApiResponse>(unauthorizedResult.Value);

            Assert.Equal("Unauthorized", response.Status);
            Assert.Null(response.Data);
            Assert.Equal("Token/Refresh Token inválidos", response.Message);
        }

        [Fact]
        public async Task RefreshToken_ReturnUnauthorized_WhenInvalidRefreshTokenExpiryTime()
        {
            var tokens = new TokenDTO
            {
                AccessToken = "test",
                RefreshToken = "test"
            };

            _authServiceMock.Setup(service => service.RefreshToken(tokens))
                .ReturnsAsync(new RepositoryResponse<TokenDTO>(RepositoryStatus.InvalidRefreshTokenExpiryTime));

            var refreshTokenResult = await _controller.RefreshToken(tokens);
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(refreshTokenResult);
            var response = Assert.IsType<ApiResponse>(unauthorizedResult.Value);

            Assert.Equal("Unauthorized", response.Status);
            Assert.Null(response.Data);
            Assert.Equal("O tempo de expiração do Refresh Token está inválido", response.Message);
        }

        [Fact]
        public async Task RefreshToken_Return500_WhenFailedToUpdateUser()
        {
            var tokens = new TokenDTO
            {
                AccessToken = "test",
                RefreshToken = "test"
            };

            _authServiceMock.Setup(service => service.RefreshToken(tokens))
               .ReturnsAsync(new RepositoryResponse<TokenDTO>(RepositoryStatus.FailedToUpdateUser));

            var refreshTokenResult = await _controller.RefreshToken(tokens);
            var objectResult = Assert.IsType<ObjectResult>(refreshTokenResult);

            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);

            var response = Assert.IsType<ApiResponse>(objectResult.Value);

            Assert.Equal("Internal Server Error", response.Status);
            Assert.Null(response.Data);
            Assert.Equal("Falha ao atualizar o usuário com o novo Refresh Token", response.Message);
        }
    }
}
