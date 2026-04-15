using System;
using System.Collections.Generic;
using System.Text;

namespace Clew.BLL.DTOs.Cart
{
    public class CartItemReadDto
    {
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string? ProductImage { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }
    }
}
