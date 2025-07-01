using Core.Enums;
using DataAccess.Entities;
using Models;


namespace DataAccess.Services
{
    public interface ITodoService
    {
        Task<TodoItem> GetByIdAsync(int id, string userId);
        Task<IEnumerable<TodoItem>> GetAllAsync(string userId);
        Task<IEnumerable<TodoItem>> GetAllAsync(string userId, int pagingNumber, int pageSize);
        Task<TodoItem> CreateAsync(TodoItemDto todo);
        Task UpdateAsync(TodoItemDto todo);
        Task DeleteAsync(int id, string userId);
        Task ToggleCompleteAsync(int id, string userId);
        Task<IEnumerable<TodoItem>> GetCompletedAsync(string userId);
        Task<IEnumerable<TodoItem>> GetByPriorityAsync(PriorityLevel priority, string userId);
    }
}
