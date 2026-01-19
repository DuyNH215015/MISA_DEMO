using MISA.Fresher.Core.DTOs.Shift;
using MISA.Fresher.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Fresher.Core.Interface.Service
{
    public interface IShiftService :  IBaseService<Shift>
    {
        /// <summary>
        /// Lấy danh sách ca làm việc theo phân trang, tìm kiếm và sắp xếp
        /// - Danh sách ca làm việc thỏa mãn điều kiện
        /// - Tổng số bản ghi
        /// </returns>
        /// CreatedBy: NHDuy
        /// CreatedDate: 10/01/2026
        Task<object> GetPagingAsync(ShiftQueryRequest request);

        int UpdateInactiveMany(List<Guid> ids, bool inactive);
    }
}
