using System;
using System.Collections.Generic;
using System.Text;
using Clew.DAL;

namespace Clew.DAL
{
    public interface ICartRepository : IGenericRepository<Cart>
    {
        Task<Cart?> GetCartByUserIdAsync(string userId);
        Task<Cart?> GetCartWithItemsAsync(string cartId);
        Task<CartItem?> GetCartItemByIdAsync(string cartItemId);
        void AddCartItem(CartItem cartItem);
        void RemoveCartItem(CartItem cartItem);
    }
}
