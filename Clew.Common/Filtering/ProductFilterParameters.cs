using System;
using System.Collections.Generic;
using System.Text;

namespace Clew.Common
{
    public class ProductFilterParameters
    {
        public string? CategoryId { get; set; }
        public string? Name { get; set; }
        public string? Material { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
