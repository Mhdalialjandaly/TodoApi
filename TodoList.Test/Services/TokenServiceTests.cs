using DataAccess.Entities;
using DataAccess.Services;
using DataAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace TodoList.Test.Services
{
    public class TokenServiceTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly Mock<ApiDbContext> _mockContext;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly TokenService _tokenService;

        public TokenServiceTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockUserManager = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            _mockContext = new Mock<ApiDbContext>();

            _mockConfiguration.Setup(x => x["JwtSettings:SecretKey"])
                .Returns("very_long_secret_key_for_testing_purposes");
            _mockConfiguration.Setup(x => x["JwtSettings:Issuer"])
                .Returns("test_issuer");
            _mockConfiguration.Setup(x => x["JwtSettings:Audience"])
                .Returns("test_audience");
            _mockConfiguration.SetupGet(x => x["JwtSettings:TokenExpiryMinutes"])
                .Returns("30");

            _tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes("very_long_secret_key_for_testing_purposes")),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true
            };

            _tokenService = new TokenService(
                _mockConfiguration.Object,
                _mockUserManager.Object,
                _mockContext.Object,
                _tokenValidationParameters);
        }

        [Fact]
        public async Task GenerateTokenAsync_ShouldReturnValidToken()
        {
            // Arrange
            var user = new User { Id = "1", Email = "test@example.com" };
            var roles = new List<string> { "User" };

            _mockUserManager.Setup(x => x.GetClaimsAsync(user))
                .ReturnsAsync(new List<Claim>());

            // Act
            var token = await _tokenService.GenerateTokenAsync(user, roles);

            // Assert
            Assert.NotNull(token);
            var tokenHandler = new JwtSecurityTokenHandler();
            Assert.True(tokenHandler.CanReadToken(token));
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldThrowException_WhenInvalidRefreshToken()
        {
            // Arrange
            var token = "invalid_token";
            var refreshToken = "invalid_refresh_token";

            _mockContext.Setup(x => x.RefreshTokens.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<RefreshToken, bool>>>(), default))
                .ReturnsAsync((RefreshToken)null);

            // Act & Assert
            await Assert.ThrowsAsync<SecurityTokenException>(() =>
                _tokenService.RefreshTokenAsync(token, refreshToken));
        }

        [Fact]
        public async Task GetPrincipalFromExpiredTokenAsync_ShouldThrowException_WhenInvalidToken()
        {
            // Arrange
            var invalidToken = "invalid_token";

            // Act & Assert
            await Assert.ThrowsAsync<SecurityTokenException>(() =>
                _tokenService.GetPrincipalFromExpiredTokenAsync(invalidToken));
        }

        [Fact]
        public async Task RevokeTokenAsync_ShouldRevokeAllTokensForUser()
        {
            // Arrange
            var userId = "1";
            var refreshTokens = new List<RefreshToken>
        {
            new RefreshToken { UserId = userId, IsRevoked = false },
            new RefreshToken { UserId = userId, IsRevoked = false }
        };

            _mockContext.Setup(x => x.RefreshTokens
                .Where(It.IsAny<Expression<Func<RefreshToken, bool>>>()))
                .Returns(refreshTokens.AsQueryable());

            // Act
            await _tokenService.RevokeTokenAsync(userId);

            // Assert
            Assert.All(refreshTokens, t => Assert.True(t.IsRevoked));
            _mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
