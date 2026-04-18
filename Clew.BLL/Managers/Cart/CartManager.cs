using AutoMapper;
using Clew.Common;
using Clew.DAL;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Clew.BLL
{
    public class CartManager :ICartManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CartManager> _logger;

        public CartManager(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<CartManager> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<GeneralResult<CartReadDto>> GetCartByUserIdAsync(string userId)
        {
            try
            {
                var cart = await _unitOfWork.Carts.GetCartByUserIdAsync(userId);

                if (cart == null)
                {
                    // Return empty cart if not exists
                    return GeneralResult<CartReadDto>.SuccessResult(new CartReadDto
                    {
                        UserId = userId,
                        CartId = string.Empty,
                        Items = new List<CartItemReadDto>()
                    });
                }

                var CartReadDto = MapToCartReadDto(cart);
                return GeneralResult<CartReadDto>.SuccessResult(CartReadDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetCartByUserIdAsync for UserId: {UserId}", userId);
                return GeneralResult<CartReadDto>.FailResult("An error occurred while retrieving the cart");
            }
        }

        public async Task<GeneralResult<CartReadDto>> AddToCartAsync(string userId, AddToCartDto addToCartReadDto)
        {
            try
            {
                // Validate product exists
                var product = await _unitOfWork.Products.GetByIdAsync(addToCartReadDto.ProductId);
                if (product == null)
                {
                    return GeneralResult<CartReadDto>.FailResult($"Product with ID {addToCartReadDto.ProductId} not found");
                }

                // Validate stock availability
                if (product.Stock < addToCartReadDto.Quantity)
                {
                    return GeneralResult<CartReadDto>.FailResult($"Only {product.Stock} items available in stock");
                }

                // Get or create cart
                var cart = await _unitOfWork.Carts.GetCartByUserIdAsync(userId);
                if (cart == null)
                {
                    cart = new Cart
                    {
                        Id = GenerateCartId(),
                        UserId = userId,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    _unitOfWork.Carts.Add(cart);
                    await _unitOfWork.SaveChangesAsync();

                    // Reload cart with items
                    cart = await _unitOfWork.Carts.GetCartWithItemsAsync(cart.Id);
                }

                // Check if product already in cart
                var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == addToCartReadDto.ProductId);

                if (existingItem != null)
                {
                    // Update quantity
                    existingItem.Quantity += addToCartReadDto.Quantity;
                    existingItem.Color = addToCartReadDto.Color ?? existingItem.Color;
                }
                else
                {
                    // Add new item
                    var cartItem = new CartItem
                    {
                        Id = GenerateCartItemId(),
                        CartId = cart.Id,
                        ProductId = addToCartReadDto.ProductId,
                        Color = addToCartReadDto.Color,
                        Quantity = addToCartReadDto.Quantity
                    };
                    _unitOfWork.Carts.AddCartItem(cartItem);
                }

                cart.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.SaveChangesAsync();

                // Get updated cart
                var updatedCart = await _unitOfWork.Carts.GetCartWithItemsAsync(cart.Id);
                var CartReadDto = MapToCartReadDto(updatedCart);

                return GeneralResult<CartReadDto>.SuccessResult(CartReadDto, "Item added to cart successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddToCartAsync for UserId: {UserId}, ProductId: {ProductId}",
                    userId, addToCartReadDto.ProductId);
                return GeneralResult<CartReadDto>.FailResult("An error occurred while adding item to cart");
            }
        }

        public async Task<GeneralResult<CartReadDto>> UpdateCartItemAsync(string userId, EditCartItemDto updateDto)
        {
            try
            {
                var cartItem = await _unitOfWork.Carts.GetCartItemByIdAsync(updateDto.CartItemId);
                if (cartItem == null)
                {
                    return GeneralResult<CartReadDto>.NotFound("Cart item not found");
                }

                // Verify cart belongs to user
                var cart = await _unitOfWork.Carts.GetCartWithItemsAsync(cartItem.CartId);
                if (cart.UserId != userId)
                {
                    return GeneralResult<CartReadDto>.FailResult("Unauthorized: Cart does not belong to user");
                }

                // Validate stock availability
                var product = await _unitOfWork.Products.GetByIdAsync(cartItem.ProductId);
                if (product.Stock < updateDto.Quantity)
                {
                    return GeneralResult<CartReadDto>.FailResult($"Only {product.Stock} items available in stock");
                }

                if (updateDto.Quantity <= 0)
                {
                    // Remove item if quantity is 0 or negative
                    _unitOfWork.Carts.RemoveCartItem(cartItem);
                }
                else
                {
                    cartItem.Quantity = updateDto.Quantity;
                }

                cart.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.SaveChangesAsync();

                // Get updated cart
                var updatedCart = await _unitOfWork.Carts.GetCartWithItemsAsync(cart.Id);
                var CartReadDto = MapToCartReadDto(updatedCart);

                return GeneralResult<CartReadDto>.SuccessResult(CartReadDto, "Cart updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateCartItemAsync for UserId: {UserId}, ItemId: {CartItemId}",
                    userId, updateDto.CartItemId);
                return GeneralResult<CartReadDto>.FailResult("An error occurred while updating cart");
            }
        }

        public async Task<GeneralResult<CartReadDto>> RemoveFromCartAsync(string userId, string cartItemId)
        {
            try
            {
                var cartItem = await _unitOfWork.Carts.GetCartItemByIdAsync(cartItemId);
                if (cartItem == null)
                {
                    return GeneralResult<CartReadDto>.NotFound("Cart item not found");
                }

                // Verify cart belongs to user
                var cart = await _unitOfWork.Carts.GetCartWithItemsAsync(cartItem.CartId);
                if (cart.UserId != userId)
                {
                    return GeneralResult<CartReadDto>.FailResult("Unauthorized: Cart does not belong to user");
                }

                _unitOfWork.Carts.RemoveCartItem(cartItem);
                cart.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.SaveChangesAsync();

                // Get updated cart
                var updatedCart = await _unitOfWork.Carts.GetCartWithItemsAsync(cart.Id);
                var CartReadDto = MapToCartReadDto(updatedCart);

                return GeneralResult<CartReadDto>.SuccessResult(CartReadDto, "Item removed from cart successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in RemoveFromCartAsync for UserId: {UserId}, ItemId: {CartItemId}",
                    userId, cartItemId);
                return GeneralResult<CartReadDto>.FailResult("An error occurred while removing item from cart");
            }
        }

        public async Task<GeneralResult<bool>> ClearCartAsync(string userId)
        {
            try
            {
                var cart = await _unitOfWork.Carts.GetCartByUserIdAsync(userId);
                if (cart == null)
                {
                    return GeneralResult<bool>.SuccessResult(true, "Cart is already empty");
                }

                // Remove all cart items
                foreach (var item in cart.Items.ToList())
                {
                    _unitOfWork.Carts.RemoveCartItem(item);
                }

                await _unitOfWork.SaveChangesAsync();

                return GeneralResult<bool>.SuccessResult(true, "Cart cleared successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ClearCartAsync for UserId: {UserId}", userId);
                return GeneralResult<bool>.FailResult("An error occurred while clearing the cart");
            }
        }

        public async Task<GeneralResult<int>> GetCartItemCountAsync(string userId)
        {
            try
            {
                var cart = await _unitOfWork.Carts.GetCartByUserIdAsync(userId);
                var count = cart?.Items.Sum(i => i.Quantity) ?? 0;
                return GeneralResult<int>.SuccessResult(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetCartItemCountAsync for UserId: {UserId}", userId);
                return GeneralResult<int>.FailResult("An error occurred while getting cart count");
            }
        }

        #region Private Helper Methods

        private CartReadDto MapToCartReadDto(Cart cart)
        {
            var items = cart.Items.Select(item => new CartItemReadDto
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductName = item.Product?.Name ?? "Unknown Product",
                ProductImage = item.Product?.Image ?? "",
                Price = item.Product?.Price ?? 0,
                Color = item.Color,
                Quantity = item.Quantity
            }).ToList();

            return new CartReadDto
            {
                CartId = cart.Id,
                UserId = cart.UserId,
                Items = items
            };
        }

        private static string GenerateCartId()
        {
            return $"cart-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}-{Guid.NewGuid().ToString().Substring(0, 6)}";
        }

        private static string GenerateCartItemId()
        {
            return Guid.NewGuid().ToString().Substring(0, 4);
        }

        #endregion
    

}
}
