using System;
using System.Collections.Generic;
using System.Text;

namespace Clew.DAL
{
    public class CartItem
    {
        public string Id { get; set; } = string.Empty;
        public string CartId { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        public string? Color { get; set; }
        public int Quantity { get; set; }

        
        public Cart Cart { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
