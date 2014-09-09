using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MvcApplication2.Models
{
    /// <summary>
    /// Data transfer object for <see cref="TodoList"/>
    /// </summary>
    public class TodoListDto
    {
        public TodoListDto() { }

        public TodoListDto(TodoList todoList)
        {
            TodoListId = todoList.TodoListId;
            UserId = todoList.UserId;
            Title = todoList.Title;
            Todos = todoList.Todos.Select(item => new TodoItemDto(item))
                .OrderBy(item => item.Priority).ToList();
        }

        [Key]
        public int TodoListId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string Title { get; set; }

        public virtual List<TodoItemDto> Todos { get; set; }

        public TodoList ToEntity()
        {
            TodoList todo = new TodoList
            {
                Title = Title,
                TodoListId = TodoListId,
                UserId = UserId,
                Todos = Todos.Select(item => item.ToEntity()).ToList()
            };

            return todo;
        }
    }
}