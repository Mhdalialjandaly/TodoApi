using AutoMapper;
using DataAccess.Entities;
using DataAccess.IRepositories;
using DataAccess.Services;
using Models;
using Moq;
using SendGrid.Helpers.Errors.Model;
using TodoList.Api.Services.Service;

namespace TodoList.Test.Services
{
    public class CategoryServiceTests
    {
        private readonly Mock<ICategoryRepository> _mockCategoryRepository;
        private readonly Mock<ICurrentUserService> _mockCurrentUserService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly CategoryService _categoryService;

        public CategoryServiceTests()
        {
            _mockCategoryRepository = new Mock<ICategoryRepository>();
            _mockCurrentUserService = new Mock<ICurrentUserService>();
            _mockMapper = new Mock<IMapper>();

            _categoryService = new CategoryService(
                _mockCategoryRepository.Object,
                _mockCurrentUserService.Object,
                _mockMapper.Object);
        }

        [Fact]
        public async Task GetCategoryByIdAsync_ShouldThrowException_WhenCategoryNotFound()
        {
            // Arrange
            var categoryId = 1;
            _mockCategoryRepository.Setup(x => x.GetByIdAsync(categoryId))
                .ReturnsAsync((Category)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _categoryService.GetCategoryByIdAsync(categoryId));
        }

        [Fact]
        public async Task GetCategoryByIdAsync_ShouldReturnCategoryDto_WhenCategoryExists()
        {
            // Arrange
            var categoryId = 1;
            var category = new Category { Id = categoryId, Name = "Test" };
            var categoryDto = new CategoryDto { Id = categoryId, Name = "Test" };

            _mockCategoryRepository.Setup(x => x.GetByIdAsync(categoryId))
                .ReturnsAsync(category);

            _mockMapper.Setup(x => x.Map<CategoryDto>(category))
                .Returns(categoryDto);

            // Act
            var result = await _categoryService.GetCategoryByIdAsync(categoryId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(categoryId, result.Id);
        }

        [Fact]
        public async Task CreateCategoryAsync_ShouldThrowException_WhenNameExists()
        {
            // Arrange
            var createDto = new CreateCategoryDto { Name = "Test" };
            _mockCategoryRepository.Setup(x => x.CategoryNameExistsAsync(createDto.Name))
                .ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() =>
                _categoryService.CreateCategoryAsync(createDto));
        }

        [Fact]
        public async Task CreateCategoryAsync_ShouldReturnCategoryDto_WhenCreationSuccessful()
        {
            // Arrange
            var createDto = new CreateCategoryDto { Name = "Test" };
            var category = new Category { Id = 1, Name = createDto.Name };
            var categoryDto = new CategoryDto { Id = 1, Name = createDto.Name };
            var userId = "user1";

            _mockCategoryRepository.Setup(x => x.CategoryNameExistsAsync(createDto.Name))
                .ReturnsAsync(false);

            _mockMapper.Setup(x => x.Map<Category>(createDto))
                .Returns(category);

            _mockCurrentUserService.Setup(x => x.UserId)
                .Returns(userId);

            _mockCategoryRepository.Setup(x => x.AddAsync(category))
                .ReturnsAsync(category);

            _mockMapper.Setup(x => x.Map<CategoryDto>(category))
                .Returns(categoryDto);

            // Act
            var result = await _categoryService.CreateCategoryAsync(createDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(categoryDto.Name, result.Name);
            Assert.Equal(userId, category.CreatedBy);
        }

        [Fact]
        public async Task UpdateCategoryAsync_ShouldThrowException_WhenCategoryNotFound()
        {
            // Arrange
            var categoryId = 1;
            var updateDto = new UpdateCategoryDto { Name = "Updated" };

            _mockCategoryRepository.Setup(x => x.GetByIdAsync(categoryId))
                .ReturnsAsync((Category)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _categoryService.UpdateCategoryAsync(categoryId, updateDto));
        }

        [Fact]
        public async Task UpdateCategoryAsync_ShouldThrowException_WhenNameExists()
        {
            // Arrange
            var categoryId = 1;
            var existingCategory = new Category { Id = categoryId, Name = "Original" };
            var updateDto = new UpdateCategoryDto { Name = "Updated" };

            _mockCategoryRepository.Setup(x => x.GetByIdAsync(categoryId))
                .ReturnsAsync(existingCategory);

            _mockCategoryRepository.Setup(x => x.CategoryNameExistsAsync(updateDto.Name))
                .ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() =>
                _categoryService.UpdateCategoryAsync(categoryId, updateDto));
        }

        [Fact]
        public async Task DeleteCategoryAsync_ShouldThrowException_WhenCategoryHasTodos()
        {
            // Arrange
            var categoryId = 1;
            var category = new Category { Id = categoryId };

            _mockCategoryRepository.Setup(x => x.GetByIdAsync(categoryId))
                .ReturnsAsync(category);

            _mockCategoryRepository.Setup(x => x.HasTodosAsync(categoryId))
                .ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() =>
                _categoryService.DeleteCategoryAsync(categoryId));
        }
    }
}
