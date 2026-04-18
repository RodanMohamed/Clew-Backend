using Clew.BLL;
using Clew.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clew.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryManager _categoryManager;

        public CategoriesController(ICategoryManager categoryManager)
        {
            _categoryManager = categoryManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _categoryManager.GetAllCategoriesAsync();
            return ToActionResult(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _categoryManager.GetCategoryByIdAsync(id);
            return ToActionResult(result);
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged([FromQuery] PaginationParameters paginationParameters, [FromQuery] string? searchTerm = null)
        {
            var result = await _categoryManager.GetCategoriesPaginationAsync(paginationParameters, searchTerm);
            return ToActionResult(result);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CategoryCreateDto dto)
        {
            var result = await _categoryManager.CreateCategoryAsync(dto);
            return ToActionResult(result);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] CategoryEditDto dto)
        {
            dto.Id = id;
            var result = await _categoryManager.EditCategoryAsync(dto);
            return ToActionResult(result);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _categoryManager.DeleteCategoryAsync(id);
            return ToActionResult(result);
        }

        private IActionResult ToActionResult<T>(GeneralResult<T> result)
        {
            if (result.Success) return Ok(result);
            if (result.Message.Contains("not found", StringComparison.OrdinalIgnoreCase)) return NotFound(result);
            if (result.Message.Contains("unauthorized", StringComparison.OrdinalIgnoreCase)) return Unauthorized(result);
            return BadRequest(result);
        }
    }
}