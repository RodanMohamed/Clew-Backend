using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Clew.DAL
{
    public class ApplicationUser:IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        
        public Address? Address { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<Favourite> Favourites { get; set; } = new List<Favourite>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
