using System;
using System.Collections.Generic;
using System.Text;

namespace Clew.BLL
{
    public class ShippingAddressDto
    {
        public string? StreetAddress { get; set; }
        public string? City { get; set; }
        public string? ZipCode { get; set; }
        public string? FullAddress { get; set; }
    }
}
