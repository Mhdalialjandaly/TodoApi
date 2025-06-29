using DataAccess.Base;
using DataAccess.Entities;
using DataAccess.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApiDbContext context) : base(context) { }

        public async Task<bool> CategoryNameExistsAsync(string name)
        {
            return await _context.Categories
            .AnyAsync(c => c.Name.ToLower() == name.ToLower());
        }

        public async Task<int> CountAsync()
        {
            return await _context.Categories.CountAsync();
        }

        public async Task<IEnumerable<Category>> GetCategoriesWithTodosAsync(string userId)
        {
            return await _context.Categories
                .Include(c => c.TodoItems.Where(t => t.UserId == userId))
                .Where(c => c.TodoItems.Any(t => t.UserId == userId))
                .ToListAsync();
        }

        public async Task<Category> GetCategoryByNameAsync(string name, string userId)
        {
            return await _context.Categories
                .Include(c => c.TodoItems.Where(t => t.UserId == userId))
                .FirstOrDefaultAsync(c => c.Name == name);
        }

        public Task<Category> GetCategoryByNameAsync(string name)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Category>> GetPopularCategoriesAsync(int count, string userId)
        {
            return await _context.Categories
                .Include(c => c.TodoItems.Where(t => t.UserId == userId))
                .OrderByDescending(c => c.TodoItems.Count(t => t.UserId == userId))
                .Take(count)
                .ToListAsync();
        }

        public Task<bool> HasTodosAsync(int categoryId)
        {
            throw new NotImplementedException();
        }
    }
}
