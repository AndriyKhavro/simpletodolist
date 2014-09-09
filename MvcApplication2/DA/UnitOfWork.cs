using MvcApplication2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcApplication2.DA
{
    public class UnitOfWork : IUnitOfWork
    {
        private TodoItemContext _context;
        private IRepository<TodoList> _todoListRepository;
        private IRepository<TodoItem> _todoItemRepository;
        private bool _disposed = false;

        public UnitOfWork()
            : this(new TodoItemContext())
        {
        }

        public UnitOfWork(TodoItemContext context)
        {
            _context = context;
        }

        public IRepository<TodoList> TodoListRepository
        {
            get
            {
                return _todoListRepository ??
                    (_todoListRepository = new Repository<TodoList>(_context));
            }
            set { _todoListRepository = value; }
        }

        public IRepository<TodoItem> TodoItemRepository
        {
            get
            {
                return _todoItemRepository ??
                    (_todoItemRepository = new TodoTaskRepository(_context));
            }
            set { _todoItemRepository = value; }
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                    GC.SuppressFinalize(this);
                }
            }
            _disposed = true;
        }
    }
}