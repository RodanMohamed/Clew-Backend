using System;
using System.Collections.Generic;
using System.Text;

namespace Clew.BLL
{
    public class PlaceOrderDto
    {
        public string PaymentMethod { get; set; } = string.Empty;
        public ShippingAddressDto ShippingAddress { get; set; } = new();
    }
}
