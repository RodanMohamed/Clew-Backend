using System;
using System.Collections.Generic;
using System.Text;

namespace Clew.DAL
{
    public class Favourite
    {
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;

    public ApplicationUser User { get; set; } = null!;
    public Product Product { get; set; } = null!;
}
}
