using System;
using System.Collections.Generic;
using System.Text;

namespace Clew.DAL
{
    public interface IAuditableEntity
    {
            public DateTime CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
    }
}
