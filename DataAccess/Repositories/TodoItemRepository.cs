using Core.Enums;
using DataAccess.Base;
using DataAccess.Entities;
using DataAccess.IRepositories;
using Microsoft.EntityFrameworkCore;
using Models.Base;

namespace DataAccess.Repositories
{
    public class TodoItemRepository : BaseRepository<TodoItem>, ITodoItemRepository
    {
        public TodoItemRepository(ApiDbContext context) : base(context) {
        }
        public async Task<IEnumerable<TodoItem>> GetByUserIdAsync(string userId) {
            return await _context.TodoItems
                .Where(t => t.UserId == userId)
                .ToListAsync();
        }

        public async Task<PagedResult<TodoItem>> GetByUserIdAsync(string userId, int pageNumber, int pageSize, string searchTerm = null) {
            var query = _context.TodoItems.Where(t => t.UserId == userId);

            if (!string.IsNullOrWhiteSpace(searchTerm)) {
                searchTerm = searchTerm.Trim().ToLower();

                query = query.Where(t =>
                    t.Title.ToLower().Contains(searchTerm) ||
                    t.Description.ToLower().Contains(searchTerm));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<TodoItem> {
                Items = items,
                TotalCount = totalCount
            };
        }


        public async Task<IEnumerable<TodoItem>> GetCompletedTodosAsync(string userId) {
            return await _context.TodoItems
                .Where(t => t.UserId == userId && t.IsCompleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<TodoItem>> GetTodosByPriorityAsync(PriorityLevel priority, string userId) {
            return await _context.TodoItems
                .Where(t => t.UserId == userId && t.Priority == priority)
                .ToListAsync();
        }
    }
}
