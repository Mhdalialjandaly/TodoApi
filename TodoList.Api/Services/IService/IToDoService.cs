using Core.Enums;
using DataAccess.Entities;


namespace DataAccess.Services
{
    public interface ITodoService
    {
        Task<TodoItem> GetByIdAsync(int id, string userId);
        Task<IEnumerable<TodoItem>> GetAllAsync(string userId);
        Task<TodoItem> CreateAsync(TodoItem todo, string userId);
        Task UpdateAsync(TodoItem todo, string userId);
        Task DeleteAsync(int id, string userId);
        Task ToggleCompleteAsync(int id, string userId);
        Task<IEnumerable<TodoItem>> GetCompletedAsync(string userId);
        Task<IEnumerable<TodoItem>> GetByPriorityAsync(PriorityLevel priority, string userId);
    }
}
