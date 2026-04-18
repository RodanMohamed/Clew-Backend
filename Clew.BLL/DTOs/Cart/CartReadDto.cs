
using System;
using System.Collections.Generic;
using System.Text;

namespace Clew.BLL
{
    public class CartReadDto
    {
        public IEnumerable<CartItemReadDto> Items { get; set; } = new List<CartItemReadDto>();

        public string CartId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public int TotalItems => Items.Sum(i => i.Quantity);
        public decimal Subtotal => Items.Sum(i => i.Total);
        public decimal Tax => Subtotal * 0.1m; 
        public decimal Total => Subtotal + Tax;
    }
}
