using MISA.Fresher.Core.DTOs.Shift;
using MISA.Fresher.Core.Entities;
using MISA.Fresher.Core.Interface.Repository;
using MISA.Fresher.Core.Interface.Service;
using MISA.Fresher.Core.Untils;
using MISA.Fresher.Core.Validators;
using System;
using System.Collections.Generic;
using MISA.Fresher.Core.Exceptions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Fresher.Core.Service
{
    public class ShiftService : BaseService<Shift>, IShiftService
    {
        private readonly IShiftRepository _shiftRepo;

        public ShiftService(IShiftRepository shiftRepo)
            : base(shiftRepo)
        {
            _shiftRepo = shiftRepo;
        }
        public async Task<object> GetPagingAsync(ShiftQueryRequest request)
        {

            ShiftQueryRequestValidator.Validate(request);
            // 2. Chuẩn hoá filter legacy (Inactive → Filters)
            NormalizeFilters(request);


            var (data, total) = await _shiftRepo.GetPagingAsync(request);
            foreach (var s in data)
            {
                s.BreakingTime = ShiftTimeCalculator.CalculateBreakingTime(
                    s.BeginBreakTime,
                    s.EndBreakTime);

                s.WorkingTime = ShiftTimeCalculator.CalculateWorkingTime(
                    s.BeginShiftTime,
                    s.EndShiftTime,
                    s.BeginBreakTime,
                    s.EndBreakTime);
            }
            return new
            {
                Total = total,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                Data = data
            };
        }
        private void NormalizeFilters(ShiftQueryRequest request)
        {
            if (request.Inactive.HasValue)
            {
                request.Filters ??= new List<FilterCondition>();

                request.Filters.Add(new FilterCondition
                {
                    Field = "Inactive",
                    Operator = "eq",
                    Value = request.Inactive.Value
                });
            }
        }

        public override Guid Create(Shift shift)
        {
            // ✅ Validate riêng cho Shift
            ShiftValidator.ValidateForInsert(shift);
            // check shiftcode 
            var isExist = _shiftRepo
           .IsShiftCodeExistAsync(shift.ShiftCode!)
           .GetAwaiter().GetResult();

            if (isExist)
                throw new ValidationException("ShiftCodeDuplicate");

            // gọi base để set audit + insert
            return base.Create(shift);
        }

        public override int Update(Shift shift)
        {
            //validate dữ liệu
            ShiftValidator.ValidateForUpdate(shift);

           var isExist = _shiftRepo
          .IsShiftCodeExistAsync(shift.ShiftCode!, shift.ShiftId)
          .GetAwaiter().GetResult();

            if (isExist)
                throw new ValidationException("ShiftCodeDuplicate");
            return base.Update(shift);
        }

        public int UpdateInactiveMany(List<Guid> ids, bool inactive)
        {
            return _shiftRepo.UpdateInactiveMany(ids, inactive);
        }
    }
}
