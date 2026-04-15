using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Clew.Common
{
    public class PaginationParameters
    {
        private const int MaxPageSize = 50;
        private int _pageSize = 10;


        [Range(1, int.MaxValue, ErrorMessage = "Page Number must be grater than 0")]
        public int PageNumber { get; set; }

        [Range(1, MaxPageSize, ErrorMessage = "Page Size must be between 1 and  50")]
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }
    }
}
