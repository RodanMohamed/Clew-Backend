using System;
using System.Collections.Generic;
using System.Text;

namespace Clew.BLL
{
    public class ProductEditDto
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
        public string CategoryId { get; set; } = string.Empty;
    }
}
