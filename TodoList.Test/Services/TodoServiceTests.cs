using Core.Enums;
using DataAccess.Entities;
using DataAccess.IRepositories;
using DataAccess.Services;
using Moq;
using SendGrid.Helpers.Errors.Model;

namespace TodoList.Test.Services
{
    public class TodoServiceTests
    {
        private readonly Mock<ITodoItemRepository> _mockTodoRepository;
        private readonly TodoService _todoService;

        public TodoServiceTests()
        {
            _mockTodoRepository = new Mock<ITodoItemRepository>();
            _todoService = new TodoService(_mockTodoRepository.Object);
        }

        [Fact]  
        public async Task GetByIdAsync_ShouldThrowException_WhenTodoNotFound()
        {
            // Arrange
            var todoId = 1;
            var userId = "user1";

            _mockTodoRepository.Setup(x => x.GetByIdAsync(todoId))
                .ReturnsAsync((TodoItem)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _todoService.GetByIdAsync(todoId, userId));
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrowException_WhenUserNotOwner()
        {
            // Arrange
            var todoId = 1;
            var userId = "user1";
            var todo = new TodoItem { Id = todoId, UserId = "user2" };

            _mockTodoRepository.Setup(x => x.GetByIdAsync(todoId))
                .ReturnsAsync(todo);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _todoService.GetByIdAsync(todoId, userId));
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnTodo_WhenCreationSuccessful()
        {
            // Arrange
            var todo = new TodoItem { Title = "Test Todo" };
            var userId = "user1";

            _mockTodoRepository.Setup(x => x.AddAsync(It.IsAny<TodoItem>()))
                .ReturnsAsync(todo);

            // Act
            var result = await _todoService.CreateAsync(todo, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(todo.Title, result.Title);
            Assert.Equal(userId, result.UserId);
        }

        [Fact]
        public async Task ToggleCompleteAsync_ShouldToggleCompletionStatus()
        {
            // Arrange
            var todoId = 1;
            var userId = "user1";
            var todo = new TodoItem { Id = todoId, UserId = userId, IsCompleted = false };

            _mockTodoRepository.Setup(x => x.GetByIdAsync(todoId))
                .ReturnsAsync(todo);

            // Act
            await _todoService.ToggleCompleteAsync(todoId, userId);

            // Assert
            Assert.True(todo.IsCompleted);
            _mockTodoRepository.Verify(x => x.UpdateAsync(todo), Times.Once);
        }

        [Fact]
        public async Task GetByPriorityAsync_ShouldReturnFilteredTodos()
        {
            // Arrange
            var priority = PriorityLevel.High;
            var userId = "user1";
            var todos = new List<TodoItem>
        {
            new TodoItem { Id = 1, UserId = userId, Priority = priority },
            new TodoItem { Id = 2, UserId = userId, Priority = priority }
        };

            _mockTodoRepository.Setup(x => x.GetTodosByPriorityAsync(priority, userId))
                .ReturnsAsync(todos);

            // Act
            var result = await _todoService.GetByPriorityAsync(priority, userId);

            // Assert
            Assert.Equal(2, result.Count());
        }
    }
}
