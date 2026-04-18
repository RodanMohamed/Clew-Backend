using System;
using System.Collections.Generic;
using System.Text;

namespace Clew.BLL { 
    public class CartItemReadDto
    {
        public string Id { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string? ProductImage { get; set; }
        public decimal Price { get; set; }
        public string? Color { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }
    }
}
