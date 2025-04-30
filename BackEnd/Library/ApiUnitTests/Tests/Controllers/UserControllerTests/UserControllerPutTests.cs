using API.Controllers;
using API.DTO.Responses;
using API.DTO.User;
using API.Enum.Responses;
using API.Repositories.Interfaces;
using ApiUnitTests.Fixtures.Controllers;
using Microsoft.AspNetCore.Http.HttpResults;
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
    public class UserControllerPutTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly UserController _controller;

        public UserControllerPutTests(UserControllerFixture fixture)
        {
            _userRepositoryMock = fixture.UserRepositoryMock;
            _controller = fixture.Controller;
        }

        [Fact]
        public async Task PutUser_ReturnNoContent()
        {
            string validId = "V4LID-ID3NTIFIER";

            var newUserInfo = new UserUpdateDTO 
            {
                Name = "valid-name",
                Email = "valid-email",
                Password = "valid-password",
                PhoneNumber = "123456789"
            };

            _userRepositoryMock.Setup(service => service.UpdateUserAsync(validId, newUserInfo))
                .ReturnsAsync(RepositoryStatus.Success);

            var putResult = await _controller.Put(validId, newUserInfo);
            Assert.IsType<NoContentResult>(putResult);
        }

        [Fact]
        public async Task PutUser_ReturnNotFound_WhenUserNotFound()
        {
            string userId = "US3R-ID";

            var newUserInfo = new UserUpdateDTO
            {
                Name = "name",
                Email = "email",
                Password = "password",
                PhoneNumber = "123456789"
            };

            _userRepositoryMock.Setup(service => service.UpdateUserAsync(userId, newUserInfo))
                .ReturnsAsync(RepositoryStatus.NotFound);

            var putResult = await _controller.Put(userId, newUserInfo);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(putResult);
            var response = Assert.IsType<ApiResponse>(notFoundResult.Value);

            Assert.Equal("Not Found", response.Status);
            Assert.Null(response.Data);
            Assert.Equal($"O usuário de id '{userId}' não foi encontrado", response.Message);
        }

        [Fact]
        public async Task PutUser_ReturnBadRequest_WhenUserIsNull()
        {
            string userId = "US3R-ID";

            var newUserInfo = new UserUpdateDTO
            {
                Name = "name",
                Email = "email",
                Password = "password",
                PhoneNumber = "123456789"
            };

            _userRepositoryMock.Setup(service => service.UpdateUserAsync(userId, newUserInfo))
                .ReturnsAsync(RepositoryStatus.NullObject);

            var putResult = await _controller.Put(userId, newUserInfo);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(putResult);
            var response = Assert.IsType<ApiResponse>(badRequestResult.Value);

            Assert.Equal("Bad Request", response.Status);
            Assert.Null(response.Data);
            Assert.Equal("O usuário não pode ser nulo", response.Message);
        }

        [Fact]
        public async Task PutUser_ReturnConflict_WhenFailedToResetPassword()
        {
            string userId = "US3R-ID";

            var newUserInfo = new UserUpdateDTO
            {
                Name = "name",
                Email = "email",
                Password = "password",
                PhoneNumber = "123456789"
            };

            _userRepositoryMock.Setup(service => service.UpdateUserAsync(userId, newUserInfo))
                .ReturnsAsync(RepositoryStatus.FailedToResetPassword);

            var putResult = await _controller.Put(userId, newUserInfo);
            var conflictResult = Assert.IsType<ConflictObjectResult>(putResult);
            var response = Assert.IsType<ApiResponse>(conflictResult.Value);

            Assert.Equal("Conflict", response.Status);
            Assert.Null(response.Data);
            Assert.Equal("Erro inesperado ao tentar atualizar a senha do usuário", response.Message);
        }

        [Fact]
        public async Task PutUser_ReturnConflict_WhenUpdateFailed()
        {
            string userId = "US3R-ID";

            var newUserInfo = new UserUpdateDTO
            {
                Name = "name",
                Email = "email",
                Password = "password",
                PhoneNumber = "123456789"
            };

            _userRepositoryMock.Setup(service => service.UpdateUserAsync(userId, newUserInfo))
                .ReturnsAsync(RepositoryStatus.Failed);

            var putResult = await _controller.Put(userId, newUserInfo);
            var conflictResult = Assert.IsType<ConflictObjectResult>(putResult);
            var response = Assert.IsType<ApiResponse>(conflictResult.Value);

            Assert.Equal("Conflict", response.Status);
            Assert.Null(response.Data);
            Assert.Equal("Erro inesperado ao tentar atualizar o usuário", response.Message);
        }

        [Fact]
        public async Task PutUserRole_ReturnNoContent_WhenOperationIsSuccessful()
        {
            string id = "US3R-ID";
            string newRole = "Librarian";

            _userRepositoryMock.Setup(service => service.UpdateUserRoleAsync(id, newRole))
                .ReturnsAsync(RepositoryStatus.Success);

            var putResult = await _controller.Put(id, newRole);
            Assert.IsType<NoContentResult>(putResult);
        }

        [Fact]
        public async Task PutUserRole_ReturnNoContent()
        {
            string id = "US3R-ID";
            string newRole = "Librarian";

            _userRepositoryMock.Setup(service => service.UpdateUserRoleAsync(id, newRole))
                .ReturnsAsync(RepositoryStatus.AlreadyInRole);

            var putResult = await _controller.Put(id, newRole);
            Assert.IsType<NoContentResult>(putResult);
        }

        [Fact]
        public async Task PutUserRole_ReturnBadRequest_WhenUserIdIsInvalid()
        {
            string invalidId = "INV4LID-ID";
            string newRole = "Librarian";

            _userRepositoryMock.Setup(service => service.UpdateUserRoleAsync(invalidId, newRole))
                .ReturnsAsync(RepositoryStatus.InvalidId);

            var putResult = await _controller.Put(invalidId, newRole);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(putResult);
            var response = Assert.IsType<ApiResponse>(badRequestResult.Value);

            Assert.Equal("Bad Request", response.Status);
            Assert.Null(response.Data);
            Assert.Equal("O id do usuário não pode ser nulo ou conter espaços em branco", response.Message);
        }

        [Fact]
        public async Task PutUserRole_ReturnBadRequest_WhenUserRoleIsInvalid()
        {
            string invalidId = "INV4LID-ID";
            string newRole = "Librarian";

            _userRepositoryMock.Setup(service => service.UpdateUserRoleAsync(invalidId, newRole))
                .ReturnsAsync(RepositoryStatus.InvalidRole);

            var putResult = await _controller.Put(invalidId, newRole);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(putResult);
            var response = Assert.IsType<ApiResponse>(badRequestResult.Value);

            Assert.Equal("Bad Request", response.Status);
            Assert.Null(response.Data);
            Assert.Equal("A role do usuário não pode ser nula ou vazia", response.Message);
        }

        [Fact]
        public async Task PutUserRole_ReturnNotFound_WhenUserNotFound()
        {
            string id = "US3R-ID";
            string newRole = "Librarian";

            _userRepositoryMock.Setup(service => service.UpdateUserRoleAsync(id, newRole))
                .ReturnsAsync(RepositoryStatus.NotFound);

            var putResult = await _controller.Put(id, newRole);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(putResult);
            var response = Assert.IsType<ApiResponse>(notFoundResult.Value);

            Assert.Equal("Not Found", response.Status);
            Assert.Null(response.Data);
            Assert.Equal($"O usuário de id '{id}' não foi encontrado", response.Message);
        }

        [Fact]
        public async Task PutUserRole_ReturnConflict_WhenFailedToRemoveRole()
        {
            string id = "US3R-ID";
            string newRole = "Librarian";

            _userRepositoryMock.Setup(service => service.UpdateUserRoleAsync(id, newRole))
                .ReturnsAsync(RepositoryStatus.RoleRemovedFailed);

            var putResult = await _controller.Put(id, newRole);
            var conflictResult = Assert.IsType<ConflictObjectResult>(putResult);
            var response = Assert.IsType<ApiResponse>(conflictResult.Value);

            Assert.Equal("Conflict", response.Status);
            Assert.Null(response.Data);
            Assert.Equal("Erro inesperado ao tentar remover a role antiga do usuário", response.Message);
        }

        [Fact]
        public async Task PutUserRole_ReturnConflict_WhenFailedToUpdateRole()
        {
            string id = "US3R-ID";
            string newRole = "Librarian";

            _userRepositoryMock.Setup(service => service.UpdateUserRoleAsync(id, newRole))
                .ReturnsAsync(RepositoryStatus.RoleUpdatedFailed);

            var putResult = await _controller.Put(id, newRole);
            var conflictResult = Assert.IsType<ConflictObjectResult>(putResult);
            var response = Assert.IsType<ApiResponse>(conflictResult.Value);

            Assert.Equal("Conflict", response.Status);
            Assert.Null(response.Data);
            Assert.Equal("Erro inesperado ao tentar atribuir o usuário a uma nova role", response.Message);
        }
    }
}
