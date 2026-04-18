using AutoMapper;
using Clew.Common;
using Clew.DAL;
using CLew.Common;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Clew.BLL
{
    public class OrderManager : IOrderManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICartManager _cartManager;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderManager> _logger;

        public OrderManager(
            IUnitOfWork unitOfWork,
            ICartManager cartManager,
            IMapper mapper,
            ILogger<OrderManager> logger)
        {
            _unitOfWork = unitOfWork;
            _cartManager = cartManager;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<GeneralResult<IEnumerable<OrderReadDto>>> GetUserOrdersAsync(string userId)
        {
            try
            {
                var orders = await _unitOfWork.Orders.GetByUserAsync(userId);
                var orderReadDtos = _mapper.Map<IEnumerable<OrderReadDto>>(orders);
                return GeneralResult<IEnumerable<OrderReadDto>>.SuccessResult(orderReadDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetUserOrdersAsync for UserId: {UserId}", userId);
                return GeneralResult<IEnumerable<OrderReadDto>>.FailResult("An error occurred while retrieving orders");
            }
        }

        public async Task<GeneralResult<OrderReadDto>> GetOrderByIdAsync(string orderId)
        {
            try
            {
                var order = await _unitOfWork.Orders.GetWithItemsAsync(orderId);
                if (order == null)
                {
                    return GeneralResult<OrderReadDto>.NotFound($"Order with ID {orderId} not found");
                }

                var orderReadDto = _mapper.Map<OrderReadDto>(order);
                return GeneralResult<OrderReadDto>.SuccessResult(orderReadDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetOrderByIdAsync for OrderId: {OrderId}", orderId);
                return GeneralResult<OrderReadDto>.FailResult("An error occurred while retrieving the order");
            }
        }

        public async Task<GeneralResult<PagedResult<OrderReadDto>>> GetAllOrdersAsync(PaginationParameters paginationParameters)
        {
            try
            {
                var allOrders = await _unitOfWork.Orders.GetAllWithItemsAsync();
                var query = allOrders.AsQueryable();

                var totalCount = query.Count();

                var items = query
                    .Skip((paginationParameters.PageNumber - 1) * paginationParameters.PageSize)
                    .Take(paginationParameters.PageSize)
                    .ToList();

                var orderReadDtos = _mapper.Map<IEnumerable<OrderReadDto>>(items);

                var pagedResult = new PagedResult<OrderReadDto>
                {
                    Items = orderReadDtos,
                    Metadata = new PaginationMetedata
                    {
                        CurrentPage = paginationParameters.PageNumber,
                        PageNumber = paginationParameters.PageNumber,
                        PageSize = paginationParameters.PageSize,
                        TotalCount = totalCount
                    }
                };

                return GeneralResult<PagedResult<OrderReadDto>>.SuccessResult(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllOrdersAsync");
                return GeneralResult<PagedResult<OrderReadDto>>.FailResult("An error occurred while retrieving orders");
            }
        }

        public async Task<GeneralResult<OrderReadDto>> CreateOrderAsync(string userId, PlaceOrderDto createOrderReadDto)
        {
            try
            {
                // Get user's cart
                var cartResult = await _cartManager.GetCartByUserIdAsync(userId);
                if (!cartResult.Success || cartResult.Data == null || !cartResult.Data.Items.Any())
                {
                    return GeneralResult<OrderReadDto>.FailResult("Cart is empty. Cannot create order.");
                }

                var cart = cartResult.Data;

                // Validate stock availability
                foreach (var item in cart.Items)
                {
                    var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
                    if (product == null)
                    {
                        return GeneralResult<OrderReadDto>.FailResult($"Product '{item.ProductName}' not found");
                    }
                    if (product.Stock < item.Quantity)
                    {
                        return GeneralResult<OrderReadDto>.FailResult($"Insufficient stock for '{item.ProductName}'. Available: {product.Stock}");
                    }
                }

                // Create order
                var order = new Order
                {
                    Id = GenerateOrderId(),
                    UserId = userId,
                    Status = "Pending",
                    PaymentMethod = createOrderReadDto.PaymentMethod,
                    Subtotal = cart.Subtotal,
                    Tax = cart.Tax,
                    Total = cart.Total,
                    ShippingAddress = new Address
                    {
                        StreetAddress = createOrderReadDto.ShippingAddress.StreetAddress,
                        City = createOrderReadDto.ShippingAddress.City,
                        ZipCode = createOrderReadDto.ShippingAddress.ZipCode,
                        FullAddress = createOrderReadDto.ShippingAddress.FullAddress
                    },
                    CreatedAt = DateTime.UtcNow,
                    EstimatedDeliveryDate = DateTime.UtcNow.AddDays(7)
                };

                // Create order items and update stock (added through association rather than non-existent methods)
                foreach (var cartItem in cart.Items)
                {
                    var orderItem = new OrderItem
                    {
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        Price = cartItem.Price
                    };
                    order.Items.Add(orderItem);

                    // Update stock
                    var product = await _unitOfWork.Products.GetByIdAsync(cartItem.ProductId);
                    if (product != null)
                    {
                        product.Stock -= cartItem.Quantity;
                        product.Status = product.Stock > 0 ? "In Stock" : "Out of Stock";
                    }
                }

                _unitOfWork.Orders.Add(order);
                await _unitOfWork.SaveChangesAsync();

                // Clear the cart
                await _cartManager.ClearCartAsync(userId);

                // Get the created order with items
                var createdOrder = await _unitOfWork.Orders.GetWithItemsAsync(order.Id);
                var orderDto = _mapper.Map<OrderReadDto>(createdOrder);

                return GeneralResult<OrderReadDto>.SuccessResult(orderDto, "Order created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateOrderAsync for UserId: {UserId}", userId);
                return GeneralResult<OrderReadDto>.FailResult("An error occurred while creating the order");
            }
        }

        public async Task<GeneralResult<OrderReadDto>> UpdateOrderStatusAsync(string orderId, string status)
        {
            try
            {
                var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
                if (order == null)
                {
                    return GeneralResult<OrderReadDto>.NotFound($"Order with ID {orderId} not found");
                }

                order.Status = status;

                if (status == "Delivered")
                {
                    order.DeliveredDate = DateTime.UtcNow;
                }

                await _unitOfWork.SaveChangesAsync();

                var updatedOrder = await _unitOfWork.Orders.GetWithItemsAsync(orderId);
                var orderReadDto = _mapper.Map<OrderReadDto>(updatedOrder);

                return GeneralResult<OrderReadDto>.SuccessResult(orderReadDto, $"Order status updated to {status}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateOrderStatusAsync for OrderId: {OrderId}, Status: {Status}", orderId, status);
                return GeneralResult<OrderReadDto>.FailResult("An error occurred while updating order status");
            }
        }

        public async Task<GeneralResult<bool>> CancelOrderAsync(string orderId, string userId)
        {
            try
            {
                var order = await _unitOfWork.Orders.GetWithItemsAsync(orderId);
                if (order == null)
                {
                    return GeneralResult<bool>.NotFound($"Order with ID {orderId} not found");
                }

                // Verify order belongs to user
                if (order.UserId != userId)
                {
                    return GeneralResult<bool>.FailResult("Unauthorized: Order does not belong to user");
                }

                // Only allow cancellation if order is pending or processing
                if (order.Status != "Pending" && order.Status != "Processing")
                {
                    return GeneralResult<bool>.FailResult($"Cannot cancel order with status '{order.Status}'");
                }

                // Restore stock
                foreach (var item in order.Items)
                {
                    var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
                    if (product != null)
                    {
                        product.Stock += item.Quantity;
                        product.Status = product.Stock > 0 ? "In Stock" : "Out of Stock";
                    }
                }

                order.Status = "Cancelled";
                await _unitOfWork.SaveChangesAsync();

                return GeneralResult<bool>.SuccessResult(true, "Order cancelled successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CancelOrderAsync for OrderId: {OrderId}, UserId: {UserId}", orderId, userId);
                return GeneralResult<bool>.FailResult("An error occurred while cancelling the order");
            }
        }

        private static string GenerateOrderId()
        {
            return $"ord-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
        }
    }
}