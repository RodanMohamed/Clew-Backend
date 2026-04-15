using Clew.BLL.DTOs.Cart;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clew.BLL
{
    public class CartReadDto
    {
        public IEnumerable<CartItemReadDto> Items { get; set; } = new List<CartItemReadDto>();
        public decimal SubTotal { get; set; }
        public int TotalItems { get; set; }
    }
}
