using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Modulos.API.Controllers;
using Modulos.API.DTOs;
using Modulos.API.Models;
using Modulos.API.Services;
using Xunit;

namespace Modulos.Tests
{
    public class AuthControllerTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly Mock<RoleManager<IdentityRole>> _mockRoleManager;
        private readonly Mock<IJwtService> _mockJwtService;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                new Mock<IUserStore<ApplicationUser>>().Object, null, null, null, null, null, null, null, null);
            _mockRoleManager = new Mock<RoleManager<IdentityRole>>(
                new Mock<IRoleStore<IdentityRole>>().Object, null, null, null, null);
            _mockJwtService = new Mock<IJwtService>();

            _controller = new AuthController(_mockUserManager.Object, _mockRoleManager.Object, _mockJwtService.Object);
        }

        [Fact]
        public async Task Login_ShouldReturnOk_WhenCredentialsAreValid()
        {
            // Arrange
            var request = new LoginRequest { Email = "test@example.com", Password = "Password123!" };
            var user = new ApplicationUser { Email = request.Email, Nombres = "Test", Apellidos = "User" };

            _mockUserManager.Setup(m => m.FindByEmailAsync(request.Email)).ReturnsAsync(user);
            _mockUserManager.Setup(m => m.CheckPasswordAsync(user, request.Password)).ReturnsAsync(true);
            _mockJwtService.Setup(j => j.GenerateToken(user)).ReturnsAsync("fake-jwt-token");
            _mockUserManager.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(new List<string> { "User" });

            // Act
            var result = await _controller.Login(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<AuthResponse>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal("fake-jwt-token", response.Token);
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
        {
            // Arrange
            var request = new LoginRequest { Email = "test@example.com", Password = "WrongPassword" };
            _mockUserManager.Setup(m => m.FindByEmailAsync(request.Email)).ReturnsAsync((ApplicationUser)null);

            // Act
            var result = await _controller.Login(request);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            var response = Assert.IsType<AuthResponse>(unauthorizedResult.Value);
            Assert.False(response.Success);
        }
    }
}
