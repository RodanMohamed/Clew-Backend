using Clew.BLL;
using Clew.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Clew.API
{
    [ApiController]
    [Route("api/orders")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderManager _orderManager;

        public OrdersController(IOrderManager orderManager)
        {
            _orderManager = orderManager;
        }

        [Authorize(Policy = "UserOnly")]
        [HttpGet]
        public async Task<IActionResult> GetUserOrders()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

            var result = await _orderManager.GetUserOrdersAsync(userId);
            return ToActionResult(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _orderManager.GetOrderByIdAsync(id);
            return ToActionResult(result);
        }

        [Authorize(Policy = "UserOnly")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PlaceOrderDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

            var result = await _orderManager.CreateOrderAsync(userId, dto);
            return ToActionResult(result);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpGet("admin")]
        public async Task<IActionResult> GetAll([FromQuery] PaginationParameters paginationParameters)
        {
            var result = await _orderManager.GetAllOrdersAsync(paginationParameters);
            return ToActionResult(result);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(string id, [FromQuery] string status)
        {
            var result = await _orderManager.UpdateOrderStatusAsync(id, status);
            return ToActionResult(result);
        }

        [Authorize(Policy = "UserOnly")]
        [HttpPatch("{id}/cancel")]
        public async Task<IActionResult> Cancel(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

            var result = await _orderManager.CancelOrderAsync(id, userId);
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