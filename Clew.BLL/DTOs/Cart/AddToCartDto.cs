using System;
using System.Collections.Generic;
using System.Text;

namespace Clew.BLL
{
    public class AddToCartDto
    {
        public string ProductId { get; set; } = string.Empty;
        public int Quantity { get; set; } = 1;
    }
}
