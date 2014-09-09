using MvcApplication2.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace MvcApplication2.Tests.Integration_tests
{
    public static class TestData
    {
        static TestData()
        {
            foreach (var item in TestData.TodoItems)
            {
                var list = TodoLists.Find(l => l.TodoListId == item.TodoListId);
                list.Todos.Add(item);
                item.TodoList = list;
            }
        }

        public static List<TodoList> TodoLists { get { return _todoLists; } }
        public static List<TodoItem> TodoItems { get { return _todoItems; } }

        private static List<TodoList> _todoLists = new List<TodoList> {
            new TodoList { TodoListId = 1, UserId = "User", Title = "Ruby courses", Todos = new List<TodoItem>() },
            new TodoList { TodoListId = 2, UserId = "User", Title = "Home", Todos = new List<TodoItem>() }
        };

        private static List<TodoItem> _todoItems = new List<TodoItem> {
            new TodoItem { TodoItemId = 1, Title = "Write HTML & CSS", TodoListId = 1, Priority = 1, Deadline = new DateTime(2014, 8, 31) }, 
            new TodoItem { TodoItemId = 2, Title = "Write unit tests", TodoListId = 1, IsDone = true, Priority = 0, Deadline = new DateTime(2014, 8, 17) },
            new TodoItem { TodoItemId = 3, Title = "Clean the room", TodoListId = 2, Priority = 2, Deadline = new DateTime(2014, 12, 31) }
        };

        public static void Add2TodoListsAnd3TodoItems(TodoItemContext dbContext)
        {
            Add2TodoListsAnd3TodoItems(dbContext.TodoLists, dbContext.TodoItems);
        }

        private static void Add2TodoListsAnd3TodoItems(IDbSet<TodoList> todoLists, IDbSet<TodoItem> todoItems)
        {
            foreach (var list in TodoLists)
            {
                todoLists.Add(list);
            }
            foreach (var item in TodoItems)
            {
                todoItems.Add(item);
            }
        }
    }
}
