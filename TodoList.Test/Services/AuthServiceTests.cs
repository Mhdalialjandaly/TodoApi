using DataAccess;
using DataAccess.Entities;
using DataAccess.Services;
using DataAccess.Services.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using SendGrid.Helpers.Errors.Model;
using TodoList.Api.Services.Service;
namespace TodoList.Test.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly Mock<RoleManager<IdentityRole>> _mockRoleManager;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly Mock<SignInManager<User>> _mockSignInManager;
        private readonly Mock<ApiDbContext> _mockContext;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _mockUserManager = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            _mockRoleManager = new Mock<RoleManager<IdentityRole>>(
                Mock.Of<IRoleStore<IdentityRole>>(), null, null, null, null);

            _mockConfiguration = new Mock<IConfiguration>();
            _mockTokenService = new Mock<ITokenService>();
            _mockSignInManager = new Mock<SignInManager<User>>(
                _mockUserManager.Object,
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<IUserClaimsPrincipalFactory<User>>(),
                null, null, null, null);

            _mockContext = new Mock<ApiDbContext>();

            _authService = new AuthService(
                _mockUserManager.Object,
                _mockContext.Object,
                _mockSignInManager.Object,
                _mockRoleManager.Object,
                _mockConfiguration.Object,
                _mockTokenService.Object);
        }


        [Fact]
        public async Task RegisterAsync_ShouldThrowException_WhenEmailExists()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Email = "test@example.com",
                LastName = "testuser",
                FirstName = "Test",
                Password = "P@ssw0rd",
                Role = "User"
            };

            _mockUserManager.Setup(x => x.FindByEmailAsync(request.Email))
                .ReturnsAsync(new User());

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() =>
                _authService.RegisterAsync(request));
        }

        [Fact]
        public async Task RegisterAsync_ShouldThrowException_WhenRoleDoesNotExist()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Email = "test@example.com",
                LastName = "testuser",
                FirstName = "Test",
                Password = "P@ssw0rd",
                Role = "User"
            };

            _mockUserManager.Setup(x => x.FindByEmailAsync(request.Email))
                .ReturnsAsync((User)null);

            _mockRoleManager.Setup(x => x.FindByNameAsync(request.Role))
                .ReturnsAsync((IdentityRole)null);

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() =>
                _authService.RegisterAsync(request));
        }

        [Fact]
        public async Task RegisterAsync_ShouldReturnAuthResponse_WhenRegistrationSuccessful()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Email = "test@example.com",
                LastName = "testuser",
                FirstName = "Test",
                Password = "P@ssw0rd",
                Role = "User"
            };

            var user = new User { Id = "1", Email = request.Email, UserName = request.UserName };
            var role = new IdentityRole(request.Role);

            _mockUserManager.Setup(x => x.FindByEmailAsync(request.Email))
                .ReturnsAsync((User)null);

            _mockRoleManager.Setup(x => x.FindByNameAsync(request.Role))
                .ReturnsAsync(role);

            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), request.Password))
                .ReturnsAsync(IdentityResult.Success);

            _mockTokenService.Setup(x => x.GenerateTokenAsync(It.IsAny<User>(), It.IsAny<List<string>>()))
                .ReturnsAsync("generated_token");

            // Act
            var result = await _authService.RegisterAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Email, result.Email);
            _mockUserManager.Verify(x => x.AddToRoleAsync(It.IsAny<User>(), role.Name), Times.Once);
        }

        [Fact]
        public async Task LoginAsync_ShouldThrowException_WhenCredentialsInvalid()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "test@example.com",
                Password = "wrongpassword"
            };

            _mockUserManager.Setup(x => x.FindByEmailAsync(request.Email))
                .ReturnsAsync(new User());

            _mockUserManager.Setup(x => x.CheckPasswordAsync(It.IsAny<User>(), request.Password))
                .ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() =>
                _authService.LoginAsync(request));
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnAuthResponse_WhenCredentialsValid()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "test@example.com",
                Password = "P@ssw0rd",
                RememberMe = false
            };

            var user = new User { Id = "1", Email = request.Email };
            var roles = new List<string> { "User" };
            var refreshToken = "refresh_token";
            var token = "access_token";

            _mockUserManager.Setup(x => x.FindByEmailAsync(request.Email))
                .ReturnsAsync(user);

            _mockUserManager.Setup(x => x.CheckPasswordAsync(user, request.Password))
                .ReturnsAsync(true);

            _mockUserManager.Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(roles);

            _mockTokenService.Setup(x => x.GenerateTokenAsync(user, roles))
                .ReturnsAsync(token);

            _mockTokenService.Setup(x => x.GenerateRefreshTokenAsync())
                .ReturnsAsync(refreshToken);

            // Act
            var result = await _authService.LoginAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Email, result.Email);
            Assert.Equal(token, result.Token);
            Assert.Equal(refreshToken, result.RefreshToken);
            _mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetCurrentUserAsync_ShouldThrowException_WhenUserNotFound()
        {
            // Arrange
            var userId = "1";
            _mockUserManager.Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _authService.GetCurrentUserAsync(userId));
        }

        [Fact]
        public async Task GetCurrentUserAsync_ShouldReturnUserResponse_WhenUserExists()
        {
            // Arrange
            var userId = "1";
            var user = new User { Id = userId, Email = "test@example.com", FullName = "Test User" };
            var roles = new List<string> { "User" };

            _mockUserManager.Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync(user);

            _mockUserManager.Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(roles);

            // Act
            var result = await _authService.GetCurrentUserAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Email, result.Email);
            Assert.Equal(user.FullName, result.FirstName);
            Assert.Equal(roles, result.Role);
        }
    }
}
