using Core.Enums;
using DataAccess.Entities;
using DataAccess.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SendGrid.Helpers.Errors.Model;
using System.Security.Claims;
using TodoList.Api.Controllers;

namespace TodoList.Test.Controllers
{

    public class TodosControllerTests
    {
        private readonly Mock<ITodoService> _mockTodoService;
        private readonly Mock<ICurrentUserService> _mockCurrentUserService;
        private readonly TodosController _controller;

        public TodosControllerTests()
        {
            _mockTodoService = new Mock<ITodoService>();
            _mockCurrentUserService = new Mock<ICurrentUserService>();
            _controller = new TodosController(_mockTodoService.Object, _mockCurrentUserService.Object);

            // Setup authenticated user by default
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "test-user-id")
            }, "TestAuthentication"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            _mockCurrentUserService.Setup(x => x.UserId).Returns("test-user-id");
        }

        [Fact]
        public async Task GetAll_ShouldReturnTodos_WhenUserAuthenticated()
        {
            // Arrange
            var expectedTodos = new List<TodoItem>
            {
                new TodoItem { Id = 1, Title = "Test Todo 1" },
                new TodoItem { Id = 2, Title = "Test Todo 2" }
            };

            _mockTodoService.Setup(x => x.GetAllAsync("test-user-id"))
                .ReturnsAsync(expectedTodos);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var todos = Assert.IsAssignableFrom<IEnumerable<TodoItem>>(okResult.Value);
            Assert.Equal(2, todos.Count());
        }

        [Fact]
        public void Test_ShouldReturnOk_WhenEndpointCalled()
        {
            // Act
            var result = _controller.Test();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Api is running", okResult.Value);
        }

        [Fact]
        public async Task GetById_ShouldReturnTodo_WhenExistsAndUserOwnsIt()
        {
            // Arrange
            var expectedTodo = new TodoItem { Id = 1, Title = "Test Todo", UserId = "test-user-id" };
            _mockTodoService.Setup(x => x.GetByIdAsync(1, "test-user-id"))
                .ReturnsAsync(expectedTodo);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var todo = Assert.IsType<TodoItem>(okResult.Value);
            Assert.Equal(expectedTodo.Title, todo.Title);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenTodoDoesNotExist()
        {
            // Arrange
            _mockTodoService.Setup(x => x.GetByIdAsync(1, "test-user-id"))
                .ThrowsAsync(new NotFoundException("Todo not found"));

            // Act
            var result = await _controller.GetById(1);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task Create_ShouldReturnCreatedAtAction_WhenSuccessful()
        {
            // Arrange
            var newTodo = new TodoItem { Title = "New Todo" };
            var createdTodo = new TodoItem { Id = 1, Title = "New Todo", UserId = "test-user-id" };

            _mockTodoService.Setup(x => x.CreateAsync(newTodo, "test-user-id"))
                .ReturnsAsync(createdTodo);

            // Act
            var result = await _controller.Create(newTodo);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(TodosController.GetById), createdAtActionResult.ActionName);
            Assert.Equal(1, createdAtActionResult.RouteValues["id"]);
            var todo = Assert.IsType<TodoItem>(createdAtActionResult.Value);
            Assert.Equal(createdTodo.Title, todo.Title);
        }

        [Fact]
        public async Task Update_ShouldReturnNoContent_WhenSuccessful()
        {
            // Arrange
            var todo = new TodoItem { Id = 1, Title = "Updated Todo", UserId = "test-user-id" };

            // Act
            var result = await _controller.Update(1, todo);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mockTodoService.Verify(x => x.UpdateAsync(todo, "test-user-id"), Times.Once);
        }

        [Fact]
        public async Task Update_ShouldReturnBadRequest_WhenIdsMismatch()
        {
            // Arrange
            var todo = new TodoItem { Id = 2, Title = "Updated Todo" };

            // Act
            var result = await _controller.Update(1, todo);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Delete_ShouldReturnNoContent_WhenSuccessful()
        {
            // Act
            var result = await _controller.Delete(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mockTodoService.Verify(x => x.DeleteAsync(1, "test-user-id"), Times.Once);
        }

        [Fact]
        public async Task ToggleComplete_ShouldReturnNoContent_WhenSuccessful()
        {
            // Act
            var result = await _controller.ToggleComplete(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mockTodoService.Verify(x => x.ToggleCompleteAsync(1, "test-user-id"), Times.Once);
        }

        [Fact]
        public async Task GetCompleted_ShouldReturnCompletedTodos_WhenUserAuthenticated()
        {
            // Arrange
            var expectedTodos = new List<TodoItem>
            {
                new TodoItem { Id = 1, Title = "Completed Todo", IsCompleted = true }
            };

            _mockTodoService.Setup(x => x.GetCompletedAsync("test-user-id"))
                .ReturnsAsync(expectedTodos);

            // Act
            var result = await _controller.GetCompleted();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var todos = Assert.IsAssignableFrom<IEnumerable<TodoItem>>(okResult.Value);
            Assert.Single(todos);
            Assert.All(todos, t => Assert.True(t.IsCompleted));
        }

        [Fact]
        public async Task GetByPriority_ShouldReturnFilteredTodos_WhenUserAuthenticated()
        {
            // Arrange
            var priority = PriorityLevel.High;
            var expectedTodos = new List<TodoItem>
            {
                new TodoItem { Id = 1, Title = "High Priority Todo", Priority = priority }
            };

            _mockTodoService.Setup(x => x.GetByPriorityAsync(priority, "test-user-id"))
                .ReturnsAsync(expectedTodos);

            // Act
            var result = await _controller.GetByPriority(priority);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var todos = Assert.IsAssignableFrom<IEnumerable<TodoItem>>(okResult.Value);
            Assert.Single(todos);
            Assert.All(todos, t => Assert.Equal(priority, t.Priority));
        }

        [Fact]
        public async Task GetAll_ShouldReturnUnauthorized_WhenUserNotAuthenticated()
        {
            // Arrange
            var unauthenticatedController = new TodosController(_mockTodoService.Object, _mockCurrentUserService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            _mockCurrentUserService.Setup(x => x.UserId).Returns((string)null);

            // Act
            var result = await unauthenticatedController.GetAll();

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }
    }
}
