using Clew.Common;
using CLew.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Clew.BLL
{
    public interface IOrderManager
    {
        Task<GeneralResult<IEnumerable<OrderReadDto>>> GetUserOrdersAsync(string userId);

        Task<GeneralResult<OrderReadDto>> GetOrderByIdAsync(string orderId);

     
        Task<GeneralResult<PagedResult<OrderReadDto>>> GetAllOrdersAsync(PaginationParameters paginationParameters);

        Task<GeneralResult<OrderReadDto>> CreateOrderAsync(string userId, PlaceOrderDto createOrderDto);
        Task<GeneralResult<OrderReadDto>> UpdateOrderStatusAsync(string orderId, string status);

        Task<GeneralResult<bool>> CancelOrderAsync(string orderId, string userId);
    }
}