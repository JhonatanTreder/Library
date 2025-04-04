using API.Controllers;
using API.Services.Interfaces;
using ApiUnitTests.Fixtures;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiUnitTests.Tests.Controllers.AuthControllerTests
{
    public class AuthControllerPutTests
    {
        private readonly AuthController _controller;
        private readonly Mock<IAuthService> _authServiceMock;

        public AuthControllerPutTests()
        {
            var fixture = new AuthServiceFixture();

            _controller = fixture.AuthController;
            _authServiceMock = fixture.MockAuthService;
        }


    }
}
