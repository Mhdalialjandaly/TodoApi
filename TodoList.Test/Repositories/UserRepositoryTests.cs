using DataAccess.Entities;
using DataAccess.Repositories;
using DataAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace TodoList.Test.Repositories
{
    public class UserRepositoryTests
    {
        private readonly Mock<ApiDbContext> _mockContext;
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly Mock<DbSet<User>> _mockUsers;
        private readonly UserRepository _repository;
        private readonly List<User> _testUsers;

        public UserRepositoryTests()
        {
            _mockContext = new Mock<ApiDbContext>();
            _mockUsers = new Mock<DbSet<User>>();
            _testUsers = GetTestUsers();

            // Setup mock DbSet
            var queryable = _testUsers.AsQueryable();
            _mockUsers.As<IQueryable<User>>().Setup(m => m.Provider).Returns(queryable.Provider);
            _mockUsers.As<IQueryable<User>>().Setup(m => m.Expression).Returns(queryable.Expression);
            _mockUsers.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            _mockUsers.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());

            // Setup mock UserManager
            var store = new Mock<IUserStore<User>>();
            _mockUserManager = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);

            // Setup mock context
            _mockContext.Setup(c => c.Users).Returns(_mockUsers.Object);

            _repository = new UserRepository(_mockContext.Object, _mockUserManager.Object);
        }

        private List<User> GetTestUsers()
        {
            return new List<User>
        {
            new User
            {
                Id = "1",
                Email = "user1@example.com",
                UserName = "user1",
                TodoItems = new List<TodoItem>
                {
                    new TodoItem { Id = 1, Title = "Task 1" },
                    new TodoItem { Id = 2, Title = "Task 2" }
                }
            },
            new User
            {
                Id = "2",
                Email = "user2@example.com",
                UserName = "user2",
                TodoItems = new List<TodoItem>()
            },
            new User
            {
                Id = "3",
                Email = "user3@example.com",
                UserName = "user3",
                TodoItems = new List<TodoItem>
                {
                    new TodoItem { Id = 3, Title = "Task 3" }
                }
            }
        };
        }

        [Fact]
        public async Task GetUserWithTodosAsync_ShouldReturnUserWithTodos()
        {
            // Arrange
            var userId = "1";
            var expectedTodoCount = 2;

            // Act
            var result = await _repository.GetUserWithTodosAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
            Assert.Equal(expectedTodoCount, result.TodoItems.Count);
        }

        [Fact]
        public async Task GetUserWithTodosAsync_ShouldReturnNull_WhenUserNotFound()
        {
            // Arrange
            var userId = "999";

            // Act
            var result = await _repository.GetUserWithTodosAsync(userId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetUserWithTodosAsync_ShouldReturnEmptyTodoList_WhenUserHasNoTodos()
        {
            // Arrange
            var userId = "2";

            // Act
            var result = await _repository.GetUserWithTodosAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.TodoItems);
        }

        [Fact]
        public async Task GetUsersByRoleAsync_ShouldReturnUsersInRole()
        {
            // Arrange
            var role = "Admin";
            var usersInRole = new List<User> { _testUsers[0], _testUsers[1] };
            _mockUserManager.Setup(x => x.GetUsersInRoleAsync(role))
                .ReturnsAsync(usersInRole);

            // Act
            var result = await _repository.GetUsersByRoleAsync(role);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(_testUsers[0], result);
            Assert.Contains(_testUsers[1], result);
        }

        [Fact]
        public async Task GetUsersByRoleAsync_ShouldReturnEmptyList_WhenNoUsersInRole()
        {
            // Arrange
            var role = "NonexistentRole";
            _mockUserManager.Setup(x => x.GetUsersInRoleAsync(role))
                .ReturnsAsync(new List<User>());

            // Act
            var result = await _repository.GetUsersByRoleAsync(role);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetUsersCountAsync_ShouldReturnCorrectCount()
        {
            // Act
            var result = await _repository.GetUsersCountAsync();

            // Assert
            Assert.Equal(_testUsers.Count, result);
        }

        [Fact]
        public async Task IsEmailUniqueAsync_ShouldReturnTrue_WhenEmailIsUnique()
        {
            // Arrange
            var newEmail = "new@example.com";

            // Act
            var result = await _repository.IsEmailUniqueAsync(newEmail);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsEmailUniqueAsync_ShouldReturnFalse_WhenEmailExists()
        {
            // Arrange
            var existingEmail = "user1@example.com";

            // Act
            var result = await _repository.IsEmailUniqueAsync(existingEmail);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsEmailUniqueAsync_ShouldBeCaseInsensitive()
        {
            // Arrange
            var existingEmail = "USER1@example.com";

            // Act
            var result = await _repository.IsEmailUniqueAsync(existingEmail);

            // Assert
            Assert.False(result);
        }
    }
}
