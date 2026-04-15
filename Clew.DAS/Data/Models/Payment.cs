using System;
using System.Collections.Generic;
using System.Text;

namespace Clew.DAL
{
    public class Payment
    {
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string CardType { get; set; } = string.Empty;
        public string CardNumber { get; set; } = string.Empty; // last 4 digits only
        public string ExpiryDate { get; set; } = string.Empty;
        public string CardholderName { get; set; } = string.Empty;
        public bool IsDefault { get; set; }

        public ApplicationUser User { get; set; } = null!;
    }
}
