using System;
using System.Collections.Generic;
using System.Text;

namespace Clew.DAL
{
    public class Cart : IAuditableEntity
    {
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }


        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
    }
}
