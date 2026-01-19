using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Fresher.Core.DTOs.Shift
{
       public class ShiftQueryRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Keyword { get; set; }
        public string? SortBy { get; set; }
        public string? SortDir { get; set; }
        public List<FilterCondition>? Filters { get; set; }

        // api mới ko dùng nữa dữ liệu vì có search đang dùng active cũ
        public bool? Inactive { get; set; }
    }
}
