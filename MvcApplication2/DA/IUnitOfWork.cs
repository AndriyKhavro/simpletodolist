using MvcApplication2.Models;
using System;
namespace MvcApplication2.DA
{
    public interface IUnitOfWork : IDisposable
    {
        void Save();
        IRepository<TodoItem> TodoItemRepository { get; set; }
        IRepository<TodoList> TodoListRepository { get; set; }
    }
}
