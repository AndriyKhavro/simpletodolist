using System;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MvcApplication2.Filters;
using MvcApplication2.Models;
using MvcApplication2.DA;

namespace MvcApplication2.Controllers
{
    [Authorize]
    [ValidateHttpAntiForgeryToken]
    public class TodoController : ApiController
    {
        private IUnitOfWork _uow;
        private IRepository<TodoItem> _todoItemRepository;
        private IRepository<TodoList> _todoListRepository;

        public TodoController()
        {
            _uow = new UnitOfWork();
            _todoItemRepository = _uow.TodoItemRepository;
            _todoListRepository = _uow.TodoListRepository;
        }

        public TodoController(IUnitOfWork uow)
        {
            _uow = uow;
            _todoItemRepository = uow.TodoItemRepository;
            _todoListRepository = uow.TodoListRepository;
        }

        // PUT api/Todo/5
        public HttpResponseMessage PutTodoItem(int id, TodoItemDto todoItemDto)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != todoItemDto.TodoItemId)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            TodoItem todoItem = todoItemDto.ToEntity();
            TodoList todoList = _todoListRepository.Find(todoItem.TodoListId);
            if (todoList == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            if (todoList.UserId != User.Identity.Name)
            {
                // Trying to modify a record that does not belong to the user
                return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }

            _todoItemRepository.Edit(todoItem);

            try
            {
                _uow.Save();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // POST api/Todo
        public HttpResponseMessage PostTodoItem(TodoItemDto todoItemDto)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            TodoList todoList = _todoListRepository.Find(todoItemDto.TodoListId);
            if (todoList == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            if (todoList.UserId != User.Identity.Name)
            {
                // Trying to add a record that does not belong to the user
                return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }

            TodoItem todoItem = todoItemDto.ToEntity();

            // Need to detach to avoid loop reference exception during JSON serialization
            _todoItemRepository.Add(todoItem);
            _uow.Save();
            todoItemDto.TodoItemId = todoItem.TodoItemId;

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, todoItemDto);
            response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = todoItemDto.TodoItemId }));
            return response;
        }

        // DELETE api/Todo/5
        public HttpResponseMessage DeleteTodoItem(int id)
        {
            TodoItem todoItem = _todoItemRepository.Find(id);
            if (todoItem == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            if (todoItem.TodoList.UserId != User.Identity.Name)
            {
                // Trying to delete a record that does not belong to the user
                return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }

            TodoItemDto todoItemDto = new TodoItemDto(todoItem);
            _todoItemRepository.Delete(todoItem);

            try
            {
                _uow.Save();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            return Request.CreateResponse(HttpStatusCode.OK, todoItemDto);
        }

        protected override void Dispose(bool disposing)
        {
            _uow.Dispose();
            base.Dispose(disposing);
        }
    }
}