using Models;

namespace DataAccess.Services
{
    public interface ICategoryService
    {
        Task<CategoryDto> GetCategoryByIdAsync(int id);
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
        Task<PaginatedResult<CategoryDto>> GetCategoriesPagedAsync(int pageNumber, int pageSize);
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto categoryDto);
        Task UpdateCategoryAsync(int id, UpdateCategoryDto categoryDto);
        Task DeleteCategoryAsync(int id);
        Task<IEnumerable<CategoryWithTodosDto>> GetCategoriesWithTodosAsync();
        Task<IEnumerable<CategoryDto>> GetPopularCategoriesAsync(int count);
        Task<bool> CategoryExistsAsync(int id);
        Task<bool> CategoryNameExistsAsync(string name);
    }
}
