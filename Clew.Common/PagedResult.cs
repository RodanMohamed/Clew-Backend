using Clew.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace CLew.Common
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = [];
        public PaginationMetedata Metadata { get; set; } = new();
    }
}
