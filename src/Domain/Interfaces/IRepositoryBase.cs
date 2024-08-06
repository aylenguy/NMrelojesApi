using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IRepositoryBase<T> where T : class
    {
        T? Get<TId>(TId id);
        List<T> Get();
        T Add(T entity);
        void Delete(T entity);
        T Update(T entity);
    }
}
