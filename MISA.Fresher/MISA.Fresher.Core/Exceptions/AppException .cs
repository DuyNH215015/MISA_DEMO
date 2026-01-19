using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Fresher.Core.Exceptions
{
    public abstract class AppException : Exception
    {
        public int StatusCode { get; }
        /// <summary>
        /// Lớp exception cơ sở của ứng dụng
        /// Dùng để định nghĩa các exception nghiệp vụ kèm theo mã HTTP StatusCode
        /// <summary>
        /// Khởi tạo một exception nghiệp vụ của ứng dụng
        /// </summary>
        /// <param name="message">
        /// Thông báo lỗi mô tả nguyên nhân exception
        /// </param>
        /// <param name="statusCode">
        /// Mã HTTP StatusCode tương ứng với loại lỗi
        /// </param>
        /// <param name="innerException">
        /// Exception gốc (nếu có) dùng để phục vụ debug
        /// </param>
        protected AppException(
            string message,
            int statusCode,
            Exception? innerException = null)
            : base(message, innerException) 
        {
            StatusCode = statusCode;
        }
    }


}
