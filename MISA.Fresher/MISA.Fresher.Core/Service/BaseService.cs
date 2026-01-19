using MISA.Fresher.Core.Exceptions;
using MISA.Fresher.Core.Interface.Repository;
using MISA.Fresher.Core.Interface.Service;
using MISA.Fresher.Core.Untils;
using System;

namespace MISA.Fresher.Core.Service
{
    /// <summary>
    /// BaseService dùng chung cho mọi entity
    /// </summary>
    public abstract class BaseService<T> : IBaseService<T>
    {
        protected readonly IBaseRepository<T> _baseRepo;

        protected BaseService(IBaseRepository<T> baseRepo)
        {
            _baseRepo = baseRepo;
        }

        /// <summary>
        /// Thêm mới bản ghi
        /// </summary>
        public virtual Guid Create(T entity)
        {
            if (entity == null)
                throw new ValidationException("Dữ liệu không hợp lệ");

            SetAuditForCreate(entity);
            return _baseRepo.Insert(entity);
        }

        /// <summary>
        /// Cập nhật bản ghi
        /// </summary>
        public virtual int Update(T entity)
        {
            if (entity == null)
                throw new ValidationException("Dữ liệu không hợp lệ");

            SetAuditForUpdate(entity);
            return _baseRepo.Update(entity);
        }

        /// <summary>
        /// Xóa 
        /// </summary>
        public virtual int Delete(Guid id)
        {
            return _baseRepo.Delete(id);
        }
        /// <summary>
        /// Xóa nhiều bản ghi 
        /// </summary>
        public int DeleteMany(List<Guid> ids)
        {
            return _baseRepo.DeleteMany(ids);
        }

        #region Audit dùng chung

        protected virtual void SetAuditForCreate(T entity)
        {
            var type = typeof(T);

            type.GetProperty("CreatedDate")?.SetValue(entity, DateTime.UtcNow);
            type.GetProperty("ModifiedDate")?.SetValue(entity, DateTime.UtcNow);

            var createdBy = type.GetProperty("CreatedBy");
            var modifiedBy = type.GetProperty("ModifiedBy");

            createdBy?.SetValue(entity,
                createdBy.GetValue(entity) ?? "system");

            modifiedBy?.SetValue(entity,
                modifiedBy.GetValue(entity) ?? createdBy?.GetValue(entity));
            var inactiveProp = type.GetProperty("Inactive");

            if (inactiveProp != null && inactiveProp.PropertyType == typeof(bool))
            {
                var currentValue = (bool)inactiveProp.GetValue(entity)!;

                if (currentValue == false)
                {
                    inactiveProp.SetValue(entity, true);
                }
            }
            SetShiftTimesIfExist(entity);
        }

        protected virtual void SetAuditForUpdate(T entity)
        {
            var type = typeof(T);

            type.GetProperty("ModifiedDate")?.SetValue(entity, DateTime.UtcNow);

            var modifiedBy = type.GetProperty("ModifiedBy");
            modifiedBy?.SetValue(entity,
                modifiedBy.GetValue(entity) ?? "system");
            SetShiftTimesIfExist(entity);
        }

        #endregion

        #region Tính giờ để gắn vào 
        protected virtual void SetShiftTimesIfExist(T entity)
        {
            var type = typeof(T);

            var beginShiftProp = type.GetProperty("BeginShiftTime");
            var endShiftProp = type.GetProperty("EndShiftTime");
            var beginBreakProp = type.GetProperty("BeginBreakTime");
            var endBreakProp = type.GetProperty("EndBreakTime");

            var workingTimeProp = type.GetProperty("WorkingTime");
            var breakingTimeProp = type.GetProperty("BreakingTime");

            // Không phải Shift → bỏ qua
            if (beginShiftProp == null || endShiftProp == null ||
                workingTimeProp == null || breakingTimeProp == null)
                return;

            var beginShift = beginShiftProp.GetValue(entity) as TimeSpan?;
            var endShift = endShiftProp.GetValue(entity) as TimeSpan?;
            var beginBreak = beginBreakProp?.GetValue(entity) as TimeSpan?;
            var endBreak = endBreakProp?.GetValue(entity) as TimeSpan?;

            // 🔥 GỌI HÀM BẠN ĐÃ VIẾT
            var breakingTime = ShiftTimeCalculator.CalculateBreakingTime(
                beginBreak,
                endBreak);

            var workingTime = ShiftTimeCalculator.CalculateWorkingTime(
                beginShift,
                endShift,
                beginBreak,
                endBreak);

            breakingTimeProp.SetValue(entity, breakingTime);
            workingTimeProp.SetValue(entity, workingTime);
        }

        #endregion
    }
}
