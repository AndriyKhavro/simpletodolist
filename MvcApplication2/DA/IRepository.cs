using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcApplication2.DA
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> GetAll(string includePath = null);
        T Find(object id);
        T Find(T entity);
        void Add(T entity);
        void Delete(T entity);
        void Delete(object id);
        void Edit(T entity);
    }
}
