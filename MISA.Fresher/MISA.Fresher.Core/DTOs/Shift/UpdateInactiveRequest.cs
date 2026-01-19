using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Fresher.Core.DTOs.Shift
{
    public class UpdateInactiveRequest
    {      
        public List<Guid> Ids { get; set; } = new();
        public bool Inactive { get; set; }
    }

}
