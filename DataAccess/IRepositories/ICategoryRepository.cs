using DataAccess.Base;
using DataAccess.Entities;

namespace DataAccess.IRepositories
{
    public interface ICategoryRepository:IBaseRepository<Category>
    {
        Task<IEnumerable<Category>> GetCategoriesWithTodosAsync(string userId);         
        Task<Category> GetCategoryByNameAsync(string name, string userId);           
        Task<IEnumerable<Category>> GetPopularCategoriesAsync(int count, string userId);
        Task<Category> GetCategoryByNameAsync(string name);
        Task<bool> CategoryNameExistsAsync(string name);
        Task<bool> HasTodosAsync(int categoryId);
        Task<int> CountAsync();
    }
}
