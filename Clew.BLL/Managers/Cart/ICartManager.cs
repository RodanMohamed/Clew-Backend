using System;
using System.Collections.Generic;
using System.Text;
using Clew.Common;
using Clew.DAL;
namespace Clew.BLL
{
    public interface ICartManager
    {
        Task<GeneralResult<CartReadDto>> GetCartByUserIdAsync(string userId);

        Task<GeneralResult<CartReadDto>> AddToCartAsync(string userId, AddToCartDto addToCartDto);

        Task<GeneralResult<CartReadDto>> UpdateCartItemAsync(string userId, EditCartItemDto updateDto);

        Task<GeneralResult<CartReadDto>> RemoveFromCartAsync(string userId, string cartItemId);

       
        Task<GeneralResult<bool>> ClearCartAsync(string userId);

        
        Task<GeneralResult<int>> GetCartItemCountAsync(string userId);
    
}
}
