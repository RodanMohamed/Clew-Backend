using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clew.DAL
{
    public class ApplicationRole : IdentityRole
    {
        public string? RoleName { get; set; }
    }
}
