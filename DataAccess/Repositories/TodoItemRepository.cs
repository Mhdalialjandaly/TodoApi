using Core.Enums;
using DataAccess.Base;
using DataAccess.Entities;
using DataAccess.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    public class TodoItemRepository : BaseRepository<TodoItem>,ITodoItemRepository
    {
        public TodoItemRepository(ApiDbContext context) : base(context)
        {
        }
        public async Task<IEnumerable<TodoItem>> GetByUserIdAsync(string userId)
        {
            return await _context.TodoItems
                .Where(t => t.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<TodoItem>> GetCompletedTodosAsync(string userId)
        {
            return await _context.TodoItems
                .Where(t => t.UserId == userId && t.IsCompleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<TodoItem>> GetTodosByPriorityAsync(PriorityLevel priority, string userId)
        {
            return await _context.TodoItems
                .Where(t => t.UserId == userId && t.Priority == priority)
                .ToListAsync();
        }
    }
}
