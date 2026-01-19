using Microsoft.AspNetCore.Mvc;
using MISA.Fresher.Core.DTOs.Shift;
using MISA.Fresher.Core.Entities;
using MISA.Fresher.Core.Interface.Service;

namespace MISA.Fresher.Api.Controllers
{
    [ApiController]
    [Route("api/shifts")]
    public class ShiftController : ControllerBase
    {
        private readonly IShiftService _shiftService;

        public ShiftController(IShiftService shiftService)
        {
            _shiftService = shiftService;
        }
        /// <summary>
        /// Truy xuất danh sách các ca làm việc được phân trang phù hợp với tiêu chí lọc đã chỉ định.
        /// </summary>
        /// <param name="request">đối tượng chứa các tham số lọc và phân trang cho truy vấn ca làm việc. Không được phép là null.</param>
        /// <returns>An <see cref="IActionResult"/> Bao gồm danh sách các ca làm việc được phân trang phù hợp với tiêu chí lọc.</returns>
        [HttpPost("filter")]
        public async Task<IActionResult> GetPaging([FromBody] ShiftQueryRequest request)
        {
            var result = await _shiftService.GetPagingAsync(request);
            return Ok(result);
        }

        #region Create

        /// <summary>
        /// Thêm mới ca làm việc
        /// </summary>
        [HttpPost]
        public IActionResult Create([FromBody] Shift shift)
        {
            var id = _shiftService.Create(shift);
            return StatusCode(201, id);
        }

        #endregion

        #region Update

        /// <summary>
        /// Cập nhật ca làm việc
        /// </summary>
        [HttpPut("{id}")]
        public IActionResult Update(Guid id, [FromBody] Shift shift)
        {
            shift.ShiftId = id;
            var affectedRows = _shiftService.Update(shift);

            if (affectedRows == 0)
                return NotFound();

            return Ok(new { Message = "Sửa thành công" });
        }

        #endregion

        #region Delete

        /// <summary>
        /// Xóa mềm ca làm việc (Inactive = true)
        /// </summary>
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var affectedRows = _shiftService.Delete(id);

            if (affectedRows == 0)
                return NotFound();

            return Ok(new { Message = "Xoá thành công" });
        }

        #endregion


        #region Inactive

        /// <summary>
        /// cập nhật trạng thái cho nhiều ca làm việc 
        /// </summary>
        [HttpPut("inactive/bulk")]
        public IActionResult UpdateInactiveMany([FromBody] UpdateInactiveRequest request)
        {
            if (request == null || request.Ids == null || !request.Ids.Any())
                return BadRequest(new { Message = "Danh sách id không hợp lệ" });

            var affectedRows = _shiftService.UpdateInactiveMany(
                request.Ids,
                request.Inactive
            );

            return Ok(new
            {
                Success = true,
                AffectedRows = affectedRows
            });
        }
        #endregion

        #region Delete bulk

        /// <summary>
        /// xóa nhiều bản ghi 
        /// </summary>
        [HttpDelete("bulk")]
        public IActionResult DeleteMany([FromBody] List<Guid> ids)
        {
            var affectedRows = _shiftService.DeleteMany(ids);

            if (affectedRows == 0)
                return BadRequest();

            return Ok(new
            {
                Message = "Xóa thành công"
            });
        }
        #endregion

    }
}