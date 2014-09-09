using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MvcApplication2.DA
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected DbContext _context;

        public Repository(DbContext context)
        {
            _context = context;
        }

        public virtual T Find(object id)
        {
            return _context.Set<T>().Find(id);
        }

        public virtual T Find(T entity)
        {
            return _context.Entry(entity).Entity;
        }

        public virtual IQueryable<T> GetAll(string includePath = null)
        {
            var entities = _context.Set<T>();
            if (!string.IsNullOrEmpty(includePath))
            {
                return entities.Include(includePath);
            }
            return entities;
        }

        public virtual void Add(T entity)
        {
            _context.Set<T>().Add(entity);
        }

        public virtual void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        public virtual void Edit(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Delete(object id)
        {
            var entity = Find(id);
            Delete(entity);
        }
    }
}