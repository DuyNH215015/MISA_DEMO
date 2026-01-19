using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Fresher.Core.Exceptions
{
    public class ValidationException : AppException
    {
        public ValidationException(string message)
            : base(message, StatusCodes.Status400BadRequest)
        {
        }
    }
}
