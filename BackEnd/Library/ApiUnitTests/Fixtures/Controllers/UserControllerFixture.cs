using API.Controllers;
using API.Repositories.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiUnitTests.Fixtures.Controllers
{
    public class UserControllerFixture
    {
        public Mock<IUserRepository> UserRepositoryMock { get; }
        public UserController Controller { get; }

        public UserControllerFixture()
        {
            UserRepositoryMock = new Mock<IUserRepository>();
            Controller = new UserController(UserRepositoryMock.Object);
        }
    }
}
