using Core.Enums;
using DataAccess.Base;
using DataAccess.Entities;

namespace DataAccess.IRepositories
{
    public interface ITodoItemRepository:IBaseRepository<TodoItem>
    {
        Task<IEnumerable<TodoItem>> GetByUserIdAsync(string userId);
        Task<IEnumerable<TodoItem>> GetCompletedTodosAsync(string userId);
        Task<IEnumerable<TodoItem>> GetTodosByPriorityAsync(PriorityLevel priority, string userId);
    }
}
