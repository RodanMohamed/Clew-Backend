using Clew.BLL;
using Clew.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Clew.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductManager _productManager;

        public ProductsController(IProductManager productManager)
        {
            _productManager = productManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _productManager.GetProductsAsync();
            return ToActionResult(result);
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged([FromQuery] PaginationParameters paginationParameters, [FromQuery] ProductFilterParameters productFilterParameters)
        {
            var result = await _productManager.GetProductsPaginationAsync(paginationParameters, productFilterParameters);
            return ToActionResult(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _productManager.GetProductByIdAsync(id);
            return ToActionResult(result);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpGet("{id}/edit")]
        public async Task<IActionResult> GetEditById(string id)
        {
            var result = await _productManager.GetProductEditByIdAsync(id);
            return ToActionResult(result);
        }

        [HttpGet("by-category/{categoryId}")]
        public async Task<IActionResult> GetByCategory(string categoryId)
        {
            var result = await _productManager.GetProductsByCategoryAsync(categoryId);
            return ToActionResult(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string searchTerm)
        {
            var result = await _productManager.SearchProductsAsync(searchTerm);
            return ToActionResult(result);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductCreateDto dto)
        {
            var result = await _productManager.CreateProductAsync(dto);
            return ToActionResult(result);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] ProductEditDto dto)
        {
            dto.Id = id;
            var result = await _productManager.EditAsync(dto);
            return ToActionResult(result);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _productManager.DeleteAsync(id);
            return ToActionResult(result);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPatch("{id}/stock")]
        public async Task<IActionResult> UpdateStock(string id, [FromQuery] int quantityChange)
        {
            var result = await _productManager.UpdateStockAsync(id, quantityChange);
            return ToActionResult(result);
        }

        [Authorize(Policy = "UserOnly")]
        [HttpPost("{id}/favorite")]
        public async Task<IActionResult> ToggleFavorite(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

            var result = await _productManager.ToggleFavoriteAsync(id, userId);
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