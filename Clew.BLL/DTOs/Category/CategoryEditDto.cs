using System;
using System.Collections.Generic;
using System.Text;

namespace Clew.BLL
{
    public class CategoryEditDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Image { get; set; }
    }
}
