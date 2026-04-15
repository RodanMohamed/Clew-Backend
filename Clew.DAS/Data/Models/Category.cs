using System;
using System.Collections.Generic;
using System.Text;

namespace Clew.DAL
{
    public class Category
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Image { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
