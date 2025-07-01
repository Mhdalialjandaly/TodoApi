using AutoMapper;
using Core.Enums;
using DataAccess;
using DataAccess.Entities;
using DataAccess.IRepositories;
using DataAccess.Services;
using Models;
using Moq;
using SendGrid.Helpers.Errors.Model;
using TodoList.Api.RequestModel;
using TodoList.Api.RequestModel.Todo;

namespace TodoList.Test.Services
{
    public class TodoServiceTests
    {
        private readonly Mock<ITodoItemRepository> _mockTodoRepository;
        private readonly IMapper _mapper;
        private readonly TodoService _todoService;

        public TodoServiceTests()
        {
            _mockTodoRepository = new Mock<ITodoItemRepository>();
            var config = new MapperConfiguration(e =>
               e.AddProfiles(new List<Profile> {
                    new SystemMapping(),
                    new RequestMappingProfile()
               }));
            _mapper = config.CreateMapper();

            _todoService = new TodoService(_mapper, _mockTodoRepository.Object);
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
            var createTodoRequest = new TodoRequestModel { Title = "New Todo", Description = "Description", Priority = PriorityLevel.Low, CategoryId = 1, IsCompleted = false };
            var todoItem = _mapper.Map<TodoItemDto>(createTodoRequest);
            todoItem.UserId = Guid.NewGuid().ToString();

            var todoEntity = _mapper.Map<TodoItem>(todoItem);

            _mockTodoRepository.Setup(x => x.AddAsync(It.IsAny<TodoItem>()))
                .ReturnsAsync(todoEntity);

            // Act
            var result = await _todoService.CreateAsync(todoItem);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(todoItem.Title, result.Title);
            Assert.Equal(todoItem.UserId, result.UserId);
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
