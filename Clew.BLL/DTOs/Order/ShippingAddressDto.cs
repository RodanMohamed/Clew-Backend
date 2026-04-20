using System;
using System.Collections.Generic;
using System.Text;

namespace Clew.BLL
{
    public class ShippingAddressDto
    {
        public string StreetAddress { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public string? FullAddress { get; set; }
    }
}
