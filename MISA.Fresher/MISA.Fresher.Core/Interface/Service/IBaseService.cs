using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MISA.Fresher.Core.Interface.Service
{
    /// <summary>
    /// Interface base cho Service
    /// Chứa các nghiệp vụ CRUD dùng chung cho mọi entity
    /// </summary>
    /// <typeparam name="T">Entity</typeparam>
    public interface IBaseService<T>
    {
        /// <summary>
        /// Thêm mới một bản ghi
        /// </summary>
        /// <param name="entity">Đối tượng cần thêm</param>
        /// <returns>Id của bản ghi vừa thêm</returns>
        Guid Create(T entity);

        /// <summary>
        /// Cập nhật một bản ghi
        /// </summary>
        /// <param name="entity">Đối tượng cần cập nhật</param>
        /// <returns>Số bản ghi bị ảnh hưởng</returns>
        int Update(T entity);

        /// <summary>
        /// Xoá một bản ghi theo Id
        /// </summary>
        /// <param name="id">Khoá chính</param>
        /// <returns>Số bản ghi bị xoá</returns>
        int Delete(Guid id);

        /// <summary>
        /// Xoá nhiều bản ghi
        /// </summary>
        int DeleteMany(List<Guid> ids);
    }
}
