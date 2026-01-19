using MISA.Fresher.Core.DTOs.Shift;
using MISA.Fresher.Core.Entities;
using MISA.Fresher.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Fresher.Core.Validators
{
    public static class ShiftQueryRequestValidator
    {
        public static void Validate(ShiftQueryRequest request)
        {
            if (request.PageNumber <= 0)
                throw new ValidationException("PageNumber phải lớn hơn 0");

            if (request.PageSize <= 0)
                throw new ValidationException("PageSize phải lớn hơn 0");

            if (request.PageSize > 100)
                throw new ValidationException("PageSize không được vượt quá 100");

            if (!string.IsNullOrEmpty(request.SortDir) &&
                request.SortDir.ToLower() is not ("asc" or "desc"))
            {
                throw new ValidationException("SortDir chỉ nhận asc hoặc desc");
            }
        }
    }

    public static class ShiftValidator
    {
        /// <summary>
        /// Validate khi thêm mới
        /// </summary>
        public static void ValidateForInsert(Shift shift)
        {
            if (string.IsNullOrWhiteSpace(shift.ShiftCode))
                throw new ValidationException("Mã ca làm việc là bắt buộc");

            if (string.IsNullOrWhiteSpace(shift.ShiftName))
                throw new ValidationException("Tên ca làm việc là bắt buộc");

            if (!shift.BeginShiftTime.HasValue)
                throw new ValidationException("Giờ bắt đầu ca là bắt buộc");

            if (!shift.EndShiftTime.HasValue)
                throw new ValidationException("Giờ kết thúc ca là bắt buộc");

            if (shift.BeginShiftTime == shift.EndShiftTime)
                throw new ValidationException("Giờ bắt đầu và giờ kết thúc không được trùng nhau");
        }

        /// <summary>
        /// Validate khi cập nhật
        /// </summary>
        public static void ValidateForUpdate(Shift shift)
        {
            if (shift.ShiftId == Guid.Empty)
                throw new ValidationException("ShiftId không hợp lệ");

            ValidateForInsert(shift);
        }
    }
}
