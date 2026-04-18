using Clew.BLL;
using Clew.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Clew.API
{
    [ApiController]
    [Route("api/cart")]
    [Authorize(Policy = "UserOnly")]
    public class CartsController : ControllerBase
    {
        private readonly ICartManager _cartManager;

        public CartsController(ICartManager cartManager)
        {
            _cartManager = cartManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

            var result = await _cartManager.GetCartByUserIdAsync(userId);
            return ToActionResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

            var result = await _cartManager.AddToCartAsync(userId, dto);
            return ToActionResult(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCartItem([FromBody] EditCartItemDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

            var result = await _cartManager.UpdateCartItemAsync(userId, dto);
            return ToActionResult(result);
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> RemoveItem(string productId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

            var cartResult = await _cartManager.GetCartByUserIdAsync(userId);
            if (!cartResult.Success || cartResult.Data == null)
            {
                return ToActionResult(cartResult);
            }

            var cartItem = cartResult.Data.Items.FirstOrDefault(i => i.ProductId == productId);
            if (cartItem == null)
            {
                return NotFound(GeneralResult<CartReadDto>.NotFound("Cart item not found for product"));
            }

            var result = await _cartManager.RemoveFromCartAsync(userId, cartItem.Id);
            return ToActionResult(result);
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

            var result = await _cartManager.ClearCartAsync(userId);
            return ToActionResult(result);
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetItemCount()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

            var result = await _cartManager.GetCartItemCountAsync(userId);
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