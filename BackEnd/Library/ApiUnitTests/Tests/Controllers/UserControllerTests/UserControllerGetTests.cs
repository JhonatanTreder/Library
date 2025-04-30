using API.Controllers;
using API.DTO.Responses;
using API.DTO.User;
using API.Enum;
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
    public class UserControllerGetTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly UserController _controller;

        public UserControllerGetTests(UserControllerFixture fixture)
        {
            _userRepositoryMock = fixture.UserRepositoryMock;
            _controller = fixture.Controller;
        }

        [Fact]
        public async Task GetUserById_ReturnOk()
        {
            string validId = "V4LID-ID";

            var user = new UserDTO
            {
                Name = "valid-name",
                Email = "valid-email",
                PhoneNumber = "123456789",
                UserType = UserType.User
            };

            _userRepositoryMock.Setup(service => service.GetUserByIdAsync(validId))
                .ReturnsAsync(new RepositoryResponse<UserDTO>(RepositoryStatus.Success, user));

            var getResult = await _controller.Get(validId);
            var okSuccessResult = Assert.IsType<OkObjectResult>(getResult);
            var response = Assert.IsType<ApiResponse>(okSuccessResult.Value);

            Assert.Equal("Ok", response.Status);
            Assert.NotNull(response.Data);
            Assert.Equal($"Usuário de id '{validId}' encontrado com sucesso", response.Message);
        }

        [Fact]
        public async Task GetUserById_ReturnBadRequest_WhenUserIdIsInvalid()
        {
            string invalidId = "INV4LID-ID";

            _userRepositoryMock.Setup(service => service.GetUserByIdAsync(invalidId))
                .ReturnsAsync(new RepositoryResponse<UserDTO>(RepositoryStatus.InvalidId));

            var getResult = await _controller.Get(invalidId);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(getResult);
            var response = Assert.IsType<ApiResponse>(badRequestResult.Value);

            Assert.Equal("Bad Request", response.Status);
            Assert.Null(response.Data);
            Assert.Equal("O id do usuário não pode ser nulo ou vazio", response.Message);
        }

        [Fact]
        public async Task GetUserById_ReturnNotFound_WhenUserNotFound()
        {
            string userId = "INV4LID-ID";

            _userRepositoryMock.Setup(service => service.GetUserByIdAsync(userId))
                .ReturnsAsync(new RepositoryResponse<UserDTO>(RepositoryStatus.UserNotFound));

            var getResult = await _controller.Get(userId);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(getResult);
            var response = Assert.IsType<ApiResponse>(notFoundResult.Value);

            Assert.Equal("Not Found", response.Status);
            Assert.Null(response.Data);
            Assert.Equal($"O usuário de id '{userId}' não foi encontrado", response.Message);
        }

        [Fact]
        public async Task GetUsers_ReturnOk()
        {
            var userFilter = new UserFilterDTO 
            {
                Name = "valid-name",
                Email = "valid-email",
                UserType = UserType.User
            };

            var users = new List<UserFilterDTO>
            {
              new UserFilterDTO(),
              new UserFilterDTO(),
              new UserFilterDTO()
            };

            _userRepositoryMock.Setup(service => service.GetUsersAsync(userFilter))
                .ReturnsAsync(new RepositoryResponse<IEnumerable<UserFilterDTO>>(RepositoryStatus.Success, users));

            var getResult = await _controller.Get(userFilter);
            var okSuccessResult = Assert.IsType<OkObjectResult>(getResult);
            var response = Assert.IsType<ApiResponse>(okSuccessResult.Value);

            Assert.Equal("Ok", response.Status);
            Assert.NotNull(response.Data);
            Assert.Equal("Usuários encontrados com sucesso", response.Message);
        }

        [Fact]
        public async Task GetUsers_ReturnBadRequest_WhenUserFilterIsNull()
        {
            var userFilter = new UserFilterDTO();

            _userRepositoryMock.Setup(service => service.GetUsersAsync(userFilter))
                .ReturnsAsync(new RepositoryResponse<IEnumerable<UserFilterDTO>>(RepositoryStatus.NullObject));

            var getResult = await _controller.Get(userFilter);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(getResult);
            var response = Assert.IsType<ApiResponse>(badRequestResult.Value);

            Assert.Equal("Bad Request", response.Status);
            Assert.Null(response.Data);
            Assert.Equal("Filtro de pesquisa inválido", response.Message);
        }

        [Fact]
        public async Task GetUsers_ReturnNotFound_WhenUsersNotFound()
        {
            var userFilter = new UserFilterDTO();

            _userRepositoryMock.Setup(service => service.GetUsersAsync(userFilter))
                .ReturnsAsync(new RepositoryResponse<IEnumerable<UserFilterDTO>>(RepositoryStatus.NotFound));

            var getResult = await _controller.Get(userFilter);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(getResult);
            var response = Assert.IsType<ApiResponse>(notFoundResult.Value);

            Assert.Equal("Not Found", response.Status);
            Assert.Null(response.Data);
            Assert.Equal("Nenhum usuário foi encontrado", response.Message);
        }
    }
}
