using API.Controllers;
using API.Repositories.Interfaces;
using ApiUnitTests.Fixtures;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiUnitTests.Tests.Controllers.UserControllerTests
{
    [Collection("UserControllerCollection")]
    public class UserControllerPostTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly UserController _controller;
        public UserControllerPostTests(UserControllerFixture fixture)
        {
            _userRepositoryMock = fixture.UserRepositoryMock;
            _controller = fixture.Controller;
        }
    }
}
