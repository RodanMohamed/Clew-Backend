using System;
using System.Collections.Generic;
using System.Text;
using Clew.Common;
using Microsoft.EntityFrameworkCore;
namespace Clew.DAL
{
        public class CartRepository : GenericRepository<Cart>, ICartRepository
        {
            public CartRepository(AppDbContext context) : base(context) { }

            public async Task<Cart?> GetCartByUserIdAsync(string userId)
            {
                return await _context.Carts
                    .Include(c => c.Items)
                    .ThenInclude(i => i.Product)
                    .FirstOrDefaultAsync(c => c.UserId == userId);
            }

            public async Task<Cart?> GetCartWithItemsAsync(string cartId)
            {
                return await _context.Carts
                    .Include(c => c.Items)
                    .ThenInclude(i => i.Product)
                    .FirstOrDefaultAsync(c => c.Id == cartId);
            }

            public async Task<CartItem?> GetCartItemByIdAsync(string cartItemId)
            {
                return await _context.CartItems
                    .Include(i => i.Product)
                    .FirstOrDefaultAsync(i => i.Id == cartItemId);
            }

            public void AddCartItem(CartItem cartItem)
            {
                _context.CartItems.Add(cartItem);
            }

            public void RemoveCartItem(CartItem cartItem)
            {
                _context.CartItems.Remove(cartItem);
            }
        }
    }

