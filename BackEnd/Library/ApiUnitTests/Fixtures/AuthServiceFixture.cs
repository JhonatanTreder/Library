using API.Controllers;
using API.Models;
using API.Services;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace ApiUnitTests.Fixtures
{
    public class AuthServiceFixture
    {
        public Mock<ITokenService> MockTokenService { get; }
        public Mock<IConfiguration> MockConfiguration { get; }
        public Mock<RoleManager<IdentityRole>> MockRoleManager { get; }
        public Mock<UserManager<ApplicationUser>> MockUserManager { get; }
        public Mock<IAuthService> MockAuthService { get; }
        public AuthService AuthService { get; }
        public AuthController AuthController { get; }

        public AuthServiceFixture()
        {
            MockConfiguration = new Mock<IConfiguration>();
            MockConfiguration.Setup(config => config["Jwt:SecretKey"])
                .Returns("super_secret_key_mock_test_128_bits");

            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            MockUserManager = new Mock<UserManager<ApplicationUser>>(
                userStoreMock.Object,
                null, null, null, null, null, null, null, null
            );

            var roleStoreMock = new Mock<IRoleStore<IdentityRole>>();
            MockRoleManager = new Mock<RoleManager<IdentityRole>>(
                roleStoreMock.Object,
                new List<IRoleValidator<IdentityRole>>(),
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<ILogger<RoleManager<IdentityRole>>>().Object
            );

            MockTokenService = new Mock<ITokenService>();

            AuthService = new AuthService(
                MockTokenService.Object,
                MockUserManager.Object,
                MockRoleManager.Object,
                MockConfiguration.Object
            );

            MockAuthService = new Mock<IAuthService>();

            AuthController = new AuthController(MockAuthService.Object);
        }
    }
}
