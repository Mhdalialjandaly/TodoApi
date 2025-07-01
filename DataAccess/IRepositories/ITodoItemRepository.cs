using Core.Enums;
using DataAccess.Base;
using DataAccess.Entities;
using Models.Base;

namespace DataAccess.IRepositories
{
    public interface ITodoItemRepository : IBaseRepository<TodoItem>
    {
        Task<IEnumerable<TodoItem>> GetByUserIdAsync(string userId);
        Task<PagedResult<TodoItem>> GetByUserIdAsync(string userId, int pageNumber, int pageSize, string searchTerm = null);
        Task<IEnumerable<TodoItem>> GetCompletedTodosAsync(string userId);
        Task<IEnumerable<TodoItem>> GetTodosByPriorityAsync(PriorityLevel priority, string userId);
    }
}
