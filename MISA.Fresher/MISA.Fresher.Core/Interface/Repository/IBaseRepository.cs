using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Fresher.Core.Interface.Repository
{
  
    public interface IBaseRepository<T>
    {
        Guid Insert(T entity);
        int Update(T entity);
        int Delete(Guid id);
        int DeleteMany(List<Guid> ids);
    }
}

