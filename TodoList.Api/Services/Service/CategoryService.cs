using AutoMapper;
using DataAccess.Entities;
using DataAccess.IRepositories;
using DataAccess.Services;
using Models;
using SendGrid.Helpers.Errors.Model;

namespace TodoList.Api.Services.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public CategoryService(
            ICategoryRepository categoryRepository,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<CategoryDto> GetCategoryByIdAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                throw new NotFoundException("Category not found");

            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }

        public async Task<PaginatedResult<CategoryDto>> GetCategoriesPagedAsync(int pageNumber, int pageSize)
        {
            var categories = await _categoryRepository.GetPagedAsync(pageNumber, pageSize);
            var totalRecords = await _categoryRepository.CountAsync();

            return new PaginatedResult<CategoryDto>
            {
                Data = _mapper.Map<IEnumerable<CategoryDto>>(categories),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize)
            };
        }

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto categoryDto)
        {
            if (await _categoryRepository.CategoryNameExistsAsync(categoryDto.Name))
                throw new BadRequestException("Category name already exists");

            var category = _mapper.Map<Category>(categoryDto);
            category.CreatedBy = _currentUserService.UserId;
            category.Created = DateTime.UtcNow;

            var createdCategory = await _categoryRepository.AddAsync(category);
            return _mapper.Map<CategoryDto>(createdCategory);
        }

        public async Task UpdateCategoryAsync(int id, UpdateCategoryDto categoryDto)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                throw new NotFoundException("Category not found");

            if (category.Name != categoryDto.Name &&
                await _categoryRepository.CategoryNameExistsAsync(categoryDto.Name))
                throw new BadRequestException("Category name already exists");

            _mapper.Map(categoryDto, category);
            category.LastModifiedBy = _currentUserService.UserId;
            category.LastModified = DateTime.UtcNow;

            await _categoryRepository.UpdateAsync(category);
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                throw new NotFoundException("Category not found");

            // Check if category has associated todos
            var hasTodos = await _categoryRepository.HasTodosAsync(id);
            if (hasTodos)
                throw new BadRequestException("Cannot delete category with associated todos");

            await _categoryRepository.DeleteAsync(category);
        }

        public async Task<IEnumerable<CategoryWithTodosDto>> GetCategoriesWithTodosAsync()
        {
            var userId = _currentUserService.UserId;
            var categories = await _categoryRepository.GetCategoriesWithTodosAsync(userId);
            return _mapper.Map<IEnumerable<CategoryWithTodosDto>>(categories);
        }

        public async Task<IEnumerable<CategoryDto>> GetPopularCategoriesAsync(int count)
        {
            var userId = _currentUserService.UserId;
            var categories = await _categoryRepository.GetPopularCategoriesAsync(count, userId);
            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }

        public async Task<bool> CategoryExistsAsync(int id)
        {
            return await _categoryRepository.Exists(id);
        }

        public async Task<bool> CategoryNameExistsAsync(string name)
        {
            return await _categoryRepository.CategoryNameExistsAsync(name);
        }
    }
}
