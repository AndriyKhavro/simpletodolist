using MvcApplication2.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MvcApplication2.DA
{
    public class TodoTaskRepository : Repository<TodoItem>
    {
        public TodoTaskRepository(DbContext context) : base(context)
        {
        }

        public override void Add(TodoItem entity)
        {
            DetachProject(entity.TodoListId);
            base.Add(entity);
        }

        public override void Edit(TodoItem entity)
        {
            DetachProject(entity.TodoListId);
            base.Edit(entity);
        }

        private void DetachProject(int id)
        {
            var project = _context.Set<TodoList>().Find(id);
            if (project == null)
            {
                throw new NotFoundException();
            }
            _context.Entry(project).State = EntityState.Detached;
        }
    }
}