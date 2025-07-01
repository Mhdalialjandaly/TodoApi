using DataAccess.Services.Response;
using DataAccess.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SendGrid.Helpers.Errors.Model;
using System.Security.Claims;
using TodoList.Api.Controllers;

namespace TodoList.Test.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mockAuthService = new Mock<IAuthService>();
            _controller = new AuthController(_mockAuthService.Object);
        }

        [Fact]
        public async Task Register_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Email", "Required");
            var request = new RegisterRequest();

            // Act
            var result = await _controller.Register(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Register_ShouldReturnAuthResponse_WhenRegistrationSuccessful()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Email = "test@example.com",
                FirstName = "Test",
                LastName = "User",
                Password = "P@ssw0rd",
                Role = "User"
            };

            var expectedResponse = new AuthResponse
            {
                Id = "1",
                Email = request.Email,
                Token = "test_token"
            };

            _mockAuthService.Setup(x => x.RegisterAsync(request))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.Register(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<AuthResponse>(okResult.Value);
            Assert.Equal(expectedResponse.Email, response.Email);
        }

        [Fact]
        public async Task Register_ShouldReturnBadRequest_WhenServiceThrowsBadRequestException()
        {
            // Arrange
            var request = new RegisterRequest();
            _mockAuthService.Setup(x => x.RegisterAsync(request))
                .ThrowsAsync(new BadRequestException("Email already exists"));

            // Act
            var result = await _controller.Register(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Email already exists", badRequestResult.Value);
        }

        [Fact]
        public async Task Login_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Email", "Required");
            var request = new LoginRequest();

            // Act
            var result = await _controller.Login(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Login_ShouldReturnAuthResponse_WhenCredentialsAreValid()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "test@example.com",
                Password = "P@ssw0rd"
            };

            var expectedResponse = new AuthResponse
            {
                Id = "1",
                Email = request.Email,
                Token = "test_token",
                RefreshToken = "refresh_token"
            };

            _mockAuthService.Setup(x => x.LoginAsync(request))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.Login(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<AuthResponse>(okResult.Value);
            Assert.Equal(expectedResponse.Token, response.Token);
        }

        [Fact]
        public async Task Login_ShouldReturnBadRequest_WhenServiceThrowsBadRequestException()
        {
            // Arrange
            var request = new LoginRequest();
            _mockAuthService.Setup(x => x.LoginAsync(request))
                .ThrowsAsync(new BadRequestException("Invalid credentials"));

            // Act
            var result = await _controller.Login(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid credentials", badRequestResult.Value);
        }

        [Fact]
        public async Task GetCurrentUser_ShouldReturnUnauthorized_WhenUserNotAuthenticated()
        {
            // Arrange
            var controller = new AuthController(_mockAuthService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            // Act
            var result = await controller.GetCurrentUser();

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task GetCurrentUser_ShouldReturnUserResponse_WhenUserIsAuthenticated()
        {
            // Arrange
            var userId = "123";
            var expectedResponse = new UserResponse
            {
                Id = userId,
                Email = "test@example.com",
                FirstName = "Test",
                Role = new List<string> { "User" }
            };

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            }, "TestAuthentication"));

            var controller = new AuthController(_mockAuthService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = user }
                }
            };

            _mockAuthService.Setup(x => x.GetCurrentUserAsync(userId))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await controller.GetCurrentUser();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<UserResponse>(okResult.Value);
            Assert.Equal(expectedResponse.Id, response.Id);
        }

        [Fact]
        public async Task GetCurrentUser_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = "123";
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            }, "TestAuthentication"));

            var controller = new AuthController(_mockAuthService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = user }
                }
            };

            _mockAuthService.Setup(x => x.GetCurrentUserAsync(userId))
                .ThrowsAsync(new NotFoundException("User not found"));

            // Act
            var result = await controller.GetCurrentUser();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("User not found", notFoundResult.Value);
        }
    }
}

