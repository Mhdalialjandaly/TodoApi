using DataAccess.Entities;
using DataAccess.Repositories;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoList.Test.Repositories
{
    public class CategoryRepositoryTests
    {
        private readonly Mock<ApiDbContext> _mockContext;
        private readonly Mock<DbSet<Category>> _mockCategories;
        private readonly CategoryRepository _repository;
        private readonly List<Category> _testCategories;

        public CategoryRepositoryTests()
        {
            _mockContext = new Mock<ApiDbContext>();
            _mockCategories = new Mock<DbSet<Category>>();
            _testCategories = GetTestCategories();

            // Setup mock DbSet
            var queryable = _testCategories.AsQueryable();
            _mockCategories.As<IQueryable<Category>>().Setup(m => m.Provider).Returns(queryable.Provider);
            _mockCategories.As<IQueryable<Category>>().Setup(m => m.Expression).Returns(queryable.Expression);
            _mockCategories.As<IQueryable<Category>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            _mockCategories.As<IQueryable<Category>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());

            // Setup mock context
            _mockContext.Setup(c => c.Categories).Returns(_mockCategories.Object);

            _repository = new CategoryRepository(_mockContext.Object);
        }

        private List<Category> GetTestCategories()
        {
            var userId = "user1";
            var otherUserId = "user2";

            return new List<Category>
        {
            new Category { Id = 1, Name = "Work", TodoItems = new List<TodoItem>
            {
                new TodoItem { Id = 1, UserId = userId },
                new TodoItem { Id = 2, UserId = userId }
            }},
            new Category { Id = 2, Name = "Personal", TodoItems = new List<TodoItem>
            {
                new TodoItem { Id = 3, UserId = userId },
                new TodoItem { Id = 4, UserId = otherUserId }
            }},
            new Category { Id = 3, Name = "Shopping", TodoItems = new List<TodoItem>()},
            new Category { Id = 4, Name = "Health", TodoItems = new List<TodoItem>
            {
                new TodoItem { Id = 5, UserId = userId }
            }},
            new Category { Id = 5, Name = "Education", TodoItems = new List<TodoItem>()}
        };
        }

        [Fact]
        public async Task CategoryNameExistsAsync_ShouldReturnTrue_WhenNameExists()
        {
            // Arrange
            var existingName = "Work";

            // Act
            var result = await _repository.CategoryNameExistsAsync(existingName);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task CategoryNameExistsAsync_ShouldReturnFalse_WhenNameDoesNotExist()
        {
            // Arrange
            var nonExistingName = "Nonexistent";

            // Act
            var result = await _repository.CategoryNameExistsAsync(nonExistingName);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task CategoryNameExistsAsync_ShouldBeCaseInsensitive()
        {
            // Arrange
            var name = "WORK";

            // Act
            var result = await _repository.CategoryNameExistsAsync(name);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task CountAsync_ShouldReturnCorrectCount()
        {
            // Act
            var result = await _repository.CountAsync();

            // Assert
            Assert.Equal(_testCategories.Count, result);
        }

        [Fact]
        public async Task GetCategoriesWithTodosAsync_ShouldReturnOnlyCategoriesWithTodosForSpecificUser()
        {
            // Arrange
            var userId = "user1";

            // Act
            var result = await _repository.GetCategoriesWithTodosAsync(userId);

            // Assert
            Assert.Equal(3, result.Count()); // Work, Personal, Health
            Assert.All(result, c => Assert.Contains(c.TodoItems, t => t.UserId == userId));
        }

        [Fact]
        public async Task GetCategoryByNameAsync_ShouldReturnCorrectCategoryWithUserTodos()
        {
            // Arrange
            var name = "Personal";
            var userId = "user1";

            // Act
            var result = await _repository.GetCategoryByNameAsync(name, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(name, result.Name);
            Assert.Single(result.TodoItems.Where(t => t.UserId == userId));
        }

        [Fact]
        public async Task GetCategoryByNameAsync_ShouldReturnNull_WhenCategoryDoesNotExist()
        {
            // Arrange
            var name = "Nonexistent";
            var userId = "user1";

            // Act
            var result = await _repository.GetCategoryByNameAsync(name, userId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetPopularCategoriesAsync_ShouldReturnTopCategoriesByTodoCount()
        {
            // Arrange
            var count = 2;
            var userId = "user1";

            // Act
            var result = await _repository.GetPopularCategoriesAsync(count, userId);

            // Assert
            Assert.Equal(count, result.Count());
            Assert.Equal("Work", result.First().Name); // Work has 2 todos
            Assert.Equal("Personal", result.Last().Name); // Personal has 1 todo (for this user)
        }

        [Fact]
        public async Task GetPopularCategoriesAsync_ShouldNotReturnCategoriesWithoutTodos()
        {
            // Arrange
            var count = 5;
            var userId = "user1";

            // Act
            var result = await _repository.GetPopularCategoriesAsync(count, userId);

            // Assert
            Assert.Equal(3, result.Count()); // Only 3 categories have todos for this user
        }

        [Fact]
        public void GetCategoryByNameAsync_WithoutUserId_ShouldThrowNotImplementedException()
        {
            // Arrange
            var name = "Work";

            // Act & Assert
            Assert.ThrowsAsync<NotImplementedException>(() => _repository.GetCategoryByNameAsync(name));
        }

        [Fact]
        public void HasTodosAsync_ShouldThrowNotImplementedException()
        {
            // Arrange
            var categoryId = 1;

            // Act & Assert
            Assert.ThrowsAsync<NotImplementedException>(() => _repository.HasTodosAsync(categoryId));
        }
    }
}
