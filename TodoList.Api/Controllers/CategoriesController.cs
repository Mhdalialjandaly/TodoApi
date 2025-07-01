using DataAccess.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using SendGrid.Helpers.Errors.Model;

namespace TodoList.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IValidator<CreateCategoryDto> _createValidator;
        private readonly IValidator<UpdateCategoryDto> _updateValidator;

        public CategoriesController(
            ICategoryService categoryService,
            IValidator<CreateCategoryDto> createValidator,
            IValidator<UpdateCategoryDto> updateValidator)
        {
            _categoryService = categoryService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAllCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDto>> GetCategoryById(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            return Ok(category);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PaginatedResult<CategoryDto>>> GetPagedCategories(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _categoryService.GetCategoriesPagedAsync(pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("with-todos")]
        public async Task<ActionResult<IEnumerable<CategoryWithTodosDto>>> GetCategoriesWithTodos()
        {
            var categories = await _categoryService.GetCategoriesWithTodosAsync();
            return Ok(categories);
        }

        [HttpGet("popular")]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetPopularCategories(
            [FromQuery] int count = 5)
        {
            var categories = await _categoryService.GetPopularCategoriesAsync(count);
            return Ok(categories);
        }

        [HttpPost]
        public async Task<ActionResult<CategoryDto>> CreateCategory(CreateCategoryDto createCategoryDto)
        {
            var validationResult = await _createValidator.ValidateAsync(createCategoryDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var category = await _categoryService.CreateCategoryAsync(createCategoryDto);
            return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, category);
        }
        [Authorize(Roles = "owner")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, UpdateCategoryDto updateCategoryDto)
        {
            var validationResult = await _updateValidator.ValidateAsync(updateCategoryDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            if (!await _categoryService.CategoryExistsAsync(id))
            {
                return NotFound();
            }

            await _categoryService.UpdateCategoryAsync(id, updateCategoryDto);
            return NoContent();
        }
        [Authorize(Roles ="owner")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            if (!await _categoryService.CategoryExistsAsync(id))
            {
                return NotFound();
            }

            try
            {
                await _categoryService.DeleteCategoryAsync(id);
                return NoContent();
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("exists/{name}")]
        public async Task<ActionResult<bool>> CheckCategoryNameExists(string name)
        {
            var exists = await _categoryService.CategoryNameExistsAsync(name);
            return Ok(exists);
        }
    }
}
