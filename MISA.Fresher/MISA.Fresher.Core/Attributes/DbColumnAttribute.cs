using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Fresher.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DbColumnAttribute : Attribute
    {
        public string Name { get; }
        /// <summary>
        /// Khởi tạo một thể hiện mới của lớp DbColumnAttribute với tên cột được chỉ định.
        /// </summary>
        /// <param name="name">Tên cột cơ sở dữ liệu được liên kết với thuộc tính hoặc trường được gán. Không được phép là null hoặc rỗng.</param>
        public DbColumnAttribute(string name)
        {
            Name = name;
        }
    }
}
