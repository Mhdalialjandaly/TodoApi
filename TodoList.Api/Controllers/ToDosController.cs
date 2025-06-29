using Core.Enums;
using DataAccess.Entities;
using DataAccess.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TodoList.Api.Controllers
{    
        [Authorize]
        [ApiController]
        [Route("api/[controller]")]
        public class TodosController : ControllerBase
        {
            private readonly ITodoService _todoService;
            private readonly ICurrentUserService _currentUserService;

            public TodosController(
                ITodoService todoService,
                ICurrentUserService currentUserService)
            {
                _todoService = todoService;
                _currentUserService = currentUserService;
            }

            [HttpGet]
            public async Task<IActionResult> GetAll()
            {
                var userId = _currentUserService.UserId;
                var todos = await _todoService.GetAllAsync(userId);
                return Ok(todos);
            }

            [HttpGet("{id}")]
            public async Task<IActionResult> GetById(int id)
            {
                var userId = _currentUserService.UserId;
                var todo = await _todoService.GetByIdAsync(id, userId);
                return Ok(todo);
            }

            [HttpPost]
            public async Task<IActionResult> Create([FromBody] TodoItem todo)
            {
                var userId = _currentUserService.UserId;
                var createdTodo = await _todoService.CreateAsync(todo, userId);
                return CreatedAtAction(nameof(GetById), new { id = createdTodo.Id }, createdTodo);
            }

            [HttpPut("{id}")]
            public async Task<IActionResult> Update(int id, [FromBody] TodoItem todo)
            {
                if (id != todo.Id)
                    return BadRequest();

                var userId = _currentUserService.UserId;
                await _todoService.UpdateAsync(todo, userId);
                return NoContent();
            }

            [HttpDelete("{id}")]
            public async Task<IActionResult> Delete(int id)
            {
                var userId = _currentUserService.UserId;
                await _todoService.DeleteAsync(id, userId);
                return NoContent();
            }

            [HttpPatch("{id}/toggle-complete")]
            public async Task<IActionResult> ToggleComplete(int id)
            {
                var userId = _currentUserService.UserId;
                await _todoService.ToggleCompleteAsync(id, userId);
                return NoContent();
            }

            [HttpGet("completed")]
            public async Task<IActionResult> GetCompleted()
            {
                var userId = _currentUserService.UserId;
                var todos = await _todoService.GetCompletedAsync(userId);
                return Ok(todos);
            }

            [HttpGet("priority/{priority}")]
            public async Task<IActionResult> GetByPriority(PriorityLevel priority)
            {
                var userId = _currentUserService.UserId;
                var todos = await _todoService.GetByPriorityAsync(priority, userId);
                return Ok(todos);
            }
        }
    
}
