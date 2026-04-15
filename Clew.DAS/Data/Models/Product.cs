using System;
using System.Collections.Generic;
using System.Text;

namespace Clew.DAL
{
    public class Product
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Image { get; set; }
        public decimal Price { get; set; }
        public string Material { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public string Status { get; set; } = "In Stock";
        public int Stock { get; set; }

        // FK
        public string CategoryId { get; set; } = string.Empty;
        public Category Category { get; set; } = null!;

        public ICollection<Favourite> Favourites { get; set; } = new List<Favourite>();
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
