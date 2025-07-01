using AutoMapper;
using Core.Enums;
using DataAccess.Entities;
using DataAccess.IRepositories;
using Models;
using SendGrid.Helpers.Errors.Model;

namespace DataAccess.Services
{
    public class TodoService : ITodoService
    {
        private readonly IMapper _mapper;
        private readonly ITodoItemRepository _todoRepository;

        public TodoService(IMapper mapper, ITodoItemRepository todoRepository) {
            _mapper = mapper;
            _todoRepository = todoRepository;
        }

        public async Task<TodoItem> GetByIdAsync(int id, string userId) {
            var todo = await _todoRepository.GetByIdAsync(id);
            if (todo == null || todo.UserId != userId)
                throw new NotFoundException("Todo not found");

            return todo;
        }

        public async Task<IEnumerable<TodoItem>> GetAllAsync(string userId) {
            return await _todoRepository.GetByUserIdAsync(userId);
        }

        public async Task<TodoItem> CreateAsync(TodoItemDto todo) {
            var dbItem = _mapper.Map<TodoItem>(todo);
            dbItem.Created = DateTime.Now;

            return await _todoRepository.AddAsync(dbItem);
        }

        public async Task UpdateAsync(TodoItemDto todo) {
            var existingTodo = await _todoRepository.GetByIdAsync(todo.Id);

            existingTodo.Title = todo.Title;
            existingTodo.Description = todo.Description;
            existingTodo.Priority = todo.Priority;
            existingTodo.IsCompleted = todo.IsCompleted;
            existingTodo.CategoryId = todo.CategoryId;
            existingTodo.LastModified = DateTime.UtcNow;

            await _todoRepository.UpdateAsync(existingTodo);
        }

        public async Task DeleteAsync(int id, string userId) {
            var todo = await GetByIdAsync(id, userId);
            await _todoRepository.DeleteAsync(todo);
        }

        public async Task ToggleCompleteAsync(int id, string userId) {
            var todo = await GetByIdAsync(id, userId);
            todo.IsCompleted = !todo.IsCompleted;
            todo.LastModified = DateTime.UtcNow;

            await _todoRepository.UpdateAsync(todo);
        }

        public async Task<IEnumerable<TodoItem>> GetCompletedAsync(string userId) {
            return await _todoRepository.GetCompletedTodosAsync(userId);
        }

        public async Task<IEnumerable<TodoItem>> GetByPriorityAsync(PriorityLevel priority, string userId) {
            return await _todoRepository.GetTodosByPriorityAsync(priority, userId);
        }
    }
}
