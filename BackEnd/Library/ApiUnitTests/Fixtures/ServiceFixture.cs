using API.Models;
using API.Services;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiUnitTests.Fixtures
{
    public class ServiceFixture
    {
        public Mock<ITokenService> MockTokenService { get; }
        public Mock<IConfiguration> MockConfiguration { get; }
        public Mock<RoleManager<IdentityRole>> MockRoleManager { get; }
        public Mock<UserManager<ApplicationUser>> MockUserManager { get; }

        public AuthService AuthService { get; }
        public TokenService TokenService { get; }

        public ServiceFixture()
        {
            MockConfiguration = new Mock<IConfiguration>();

            MockConfiguration.Setup(config => config["Jwt:SecretKey"])
                .Returns("super_secret_key_mock_test_128_bytes");

            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            MockUserManager = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object);

            var roleStoreMock = new Mock<IRoleStore<IdentityRole>>();
            MockRoleManager = new Mock<RoleManager<IdentityRole>>(roleStoreMock.Object);

            MockTokenService = new Mock<ITokenService>();

            TokenService = new TokenService(MockConfiguration.Object);
            AuthService = new AuthService(MockTokenService.Object, MockUserManager.Object,
                                         MockRoleManager.Object, MockConfiguration.Object);

        }
    }
}
