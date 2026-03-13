using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using Modulos.API.Models;
using Modulos.API.Services;
using System.Security.Claims;
using Xunit;

namespace Modulos.Tests
{
    public class JwtServiceTests
    {
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly JwtService _jwtService;

        public JwtServiceTests()
        {
            _mockConfig = new Mock<IConfiguration>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                new Mock<IUserStore<ApplicationUser>>().Object, null, null, null, null, null, null, null, null);

            _jwtService = new JwtService(_mockConfig.Object, _mockUserManager.Object);
        }

        [Fact]
        public async Task GenerateToken_ShouldReturnToken_WhenUserIsValid()
        {
            // Arrange
            var user = new ApplicationUser
            {
                Id = "1",
                Email = "test@example.com",
                Nombres = "Test",
                Apellidos = "User"
            };

            _mockConfig.Setup(c => c["Jwt:Key"]).Returns("ThisIsAStrongSecretKeyForTestingPurposes!");
            _mockConfig.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
            _mockConfig.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");
            _mockUserManager.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(new List<string> { "User" });

            // Act
            var token = await _jwtService.GenerateToken(user);

            // Assert
            Assert.NotNull(token);
            Assert.NotEmpty(token);
        }
    }
}
