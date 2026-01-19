using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MISA.Fresher.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MISA.Fresher.Core.Middleware
{
    /// <summary>
    /// Middleware dùng để bắt và xử lý exception toàn cục trong ứng dụng
    /// </summary>
    public class ExceptionMiddleware
    {  
        /// <summary>
       /// Delegate đại diện cho middleware tiếp theo trong pipeline
       /// </summary>
        private readonly RequestDelegate _next;
        /// <summary>
        /// Logger dùng để ghi log khi xảy ra exception
        /// </summary>
        private readonly ILogger<ExceptionMiddleware> _logger;


        /// <summary>
        /// Khởi tạo ExceptionMiddleware
        /// </summary>
        /// <param name="next">
        /// Middleware tiếp theo trong pipeline
        /// </param>
        /// <param name="logger">
        /// Logger phục vụ việc ghi log exception
        /// </param>
        public ExceptionMiddleware(
            RequestDelegate next,
            ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Phương thức được gọi cho mỗi HTTP request
        /// Dùng để bắt và xử lý exception phát sinh trong quá trình xử lý request
        /// </summary>
        /// <param name="context">
        /// HttpContext của request hiện tại
        /// </param>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (AppException ex)
            {
                _logger.LogWarning(ex, ex.Message);
                await WriteResponse(context, ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                await WriteResponse(
                    context,
                    StatusCodes.Status500InternalServerError,
                    "Có lỗi hệ thống xảy ra"
                );
            }
        }

        /// <summary>
        /// Ghi response lỗi trả về cho client dưới dạng JSON
        /// </summary>
        /// <param name="context">
        /// HttpContext hiện tại
        /// </param>
        /// <param name="statusCode">
        /// Mã HTTP StatusCode trả về
        /// </param>
        /// <param name="message">
        /// Thông báo lỗi gửi cho client
        /// </param>
        private static async Task WriteResponse(
            HttpContext context,
            int statusCode,
            string message)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var response = new
            {
                success = false,
                message
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
