using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Fresher.Core.Exceptions
{
    public class RepositoryException : AppException
    {
        public RepositoryException(string message, Exception innerException)
            : base(message, StatusCodes.Status500InternalServerError, innerException)
        {
        }
    }

}
