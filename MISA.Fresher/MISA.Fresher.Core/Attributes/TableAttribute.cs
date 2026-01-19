using System;

namespace MISA.Fresher.Core.Attributes
{
    /// <summary>
    /// Attribute dùng để khai báo tên bảng trong database
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute : Attribute
    {
        /// <summary>
        /// Tên bảng trong database
        /// </summary>
        public string Name { get; }

        public TableAttribute(string name)
        {
            Name = name;
        }
    }
}
