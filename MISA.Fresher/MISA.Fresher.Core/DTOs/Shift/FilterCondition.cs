using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Fresher.Core.DTOs.Shift
{
    public class FilterCondition
    {
        public string Field { get; set; } = string.Empty;   // shiftCode, shiftName...
        public string Operator { get; set; } = string.Empty; // contains, equals...
        public object? Value { get; set; }                  // "CA01", true, date...
    }
}