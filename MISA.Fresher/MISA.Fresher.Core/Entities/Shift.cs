using MISA.Fresher.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Fresher.Core.Entities
{
    [Table("shift")]
    public class Shift
    {
        [DbColumn("shift_id")]
        public Guid ShiftId { get; set; }

        [DbColumn("shift_code")]
        public string? ShiftCode { get; set; }
        [DbColumn("shift_name")]
        public string? ShiftName { get; set; }
        [DbColumn("begin_shift_time")]
        public TimeSpan? BeginShiftTime { get; set; }
        [DbColumn("end_shift_time")]
        public TimeSpan? EndShiftTime { get; set; }
        [DbColumn("begin_break_time")]
        public TimeSpan? BeginBreakTime { get; set; }
        [DbColumn("end_break_time")]
        public TimeSpan? EndBreakTime { get; set; }

        [DbColumn("working_time")]
        public decimal WorkingTime { get; set; }
        [DbColumn("breaking_time")]
        public decimal BreakingTime { get; set; }
        [DbColumn("description")]
        public string? Description { get; set; }
        [DbColumn("inactive")]

        public bool Inactive { get; set; }
        [DbColumn("created_by")]
        public string? CreatedBy { get; set; }

        [DbColumn("created_date")]
        public DateTime CreatedDate { get; set; }
        [DbColumn("modified_by")]
        public string? ModifiedBy { get; set; }

        [DbColumn("modified_date")]
        public DateTime ModifiedDate { get; set; }
    }
}
