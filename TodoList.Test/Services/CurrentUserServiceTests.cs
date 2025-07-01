using DataAccess.Entities;
using DataAccess.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using System.Security.Claims;

namespace TodoList.Test.Services
{
    public class CurrentUserServiceTests
    {
        [Fact]
        public void UserId_ShouldReturnClaimValue_WhenUserIsAuthenticated()
        {
            // Arrange
            var userId = "123";
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.NameIdentifier, userId)
        }));

            var httpContext = new DefaultHttpContext { User = user };
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

            var userManager = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var service = new CurrentUserService(httpContextAccessor.Object, userManager.Object);

            // Act
            var result = service.UserId;

            // Assert
            Assert.Equal(userId, result);
        }

        [Fact]
        public async Task GetCurrentUserAsync_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var userId = "123";
            var user = new User { Id = userId };

            var httpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.NameIdentifier, userId)
            }))
            };

            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

            var userManager = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            userManager.Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync(user);

            var service = new CurrentUserService(httpContextAccessor.Object, userManager.Object);

            // Act
            var result = await service.GetCurrentUserAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
        }

        [Fact]
        public void Role_ShouldReturnClaimValue_WhenRoleExists()
        {
            // Arrange
            var role = "Admin";
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.Role, role)
        }));

            var httpContext = new DefaultHttpContext { User = user };
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

            var userManager = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var service = new CurrentUserService(httpContextAccessor.Object, userManager.Object);

            // Act
            var result = service.Role;

            // Assert
            Assert.Equal(role, result);
        }
    }
}
