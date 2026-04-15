using System;
using System.Collections.Generic;
using System.Text;

namespace Clew.BLL
{
    public class EditCartItemDto
    {
        public string ProductId { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}
