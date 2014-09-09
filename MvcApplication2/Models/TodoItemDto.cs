using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace MvcApplication2.Models
{
    public class TodoItemDto
    {
        /// <summary>
        /// Data transfer object for <see cref="TodoItem"/>
        /// </summary>
        public TodoItemDto() { }

        public TodoItemDto(TodoItem item)
        {
            TodoItemId = item.TodoItemId;
            Title = item.Title;
            IsDone = item.IsDone;
            Deadline = item.Deadline;
            Priority = item.Priority;
            TodoListId = item.TodoListId;
        }

        [Key]
        public int TodoItemId { get; set; }

        [Required]
        public string Title { get; set; }

        public bool IsDone { get; set; }
        public DateTime? Deadline { get; set; }
        public int Priority { get; set; }

        public int TodoListId { get; set; }

        public TodoItem ToEntity()
        {
            return new TodoItem
            {
                TodoItemId = TodoItemId,
                Title = Title,
                IsDone = IsDone,
                Deadline = Deadline == DateTime.MinValue ? (DateTime?) null : Deadline,
                Priority = Priority,
                TodoListId = TodoListId
            };
        }
    }
}
