using System;
using System.Collections.Generic;
using System.Text;

namespace Clew.DAL
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetByUserAsync(string userId);
        Task<Order?> GetWithItemsAsync(string orderId);
        Task<IEnumerable<Order>> GetAllWithItemsAsync();
    }
}
