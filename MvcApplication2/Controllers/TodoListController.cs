using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MvcApplication2.Filters;
using MvcApplication2.Models;
using MvcApplication2.DA;

namespace MvcApplication2.Controllers
{
    [Authorize]
    public class TodoListController : ApiController
    {
        private IUnitOfWork _uow;
        private IRepository<TodoList> _todoListRepository;

        public TodoListController()
        {
            _uow = new UnitOfWork();
            _todoListRepository = _uow.TodoListRepository;
        }

        public TodoListController(IUnitOfWork uow)
        {
            _uow = uow;
            _todoListRepository = uow.TodoListRepository;
        }

        // GET api/TodoList
        public IEnumerable<TodoListDto> GetTodoLists()
        {
            return _todoListRepository.GetAll("Todos")
                .Where(u => u.UserId == User.Identity.Name)
                .OrderBy(u => u.TodoListId)
                .AsEnumerable()
                .Select(todoList => new TodoListDto(todoList));
        }

        // GET api/TodoList/5
        public TodoListDto GetTodoList(int id)
        {
            TodoList todoList = _todoListRepository.Find(id);
            if (todoList == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            if (todoList.UserId != User.Identity.Name)
            {
                // Trying to modify a record that does not belong to the user
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.Unauthorized));
            }

            return new TodoListDto(todoList);
        }

        // PUT api/TodoList/5
        [ValidateHttpAntiForgeryToken]
        public HttpResponseMessage PutTodoList(int id, TodoListDto todoListDto)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != todoListDto.TodoListId)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            TodoList todoList = todoListDto.ToEntity();
            TodoList todoListInDb = _todoListRepository.Find(todoList);
            if (todoListInDb.UserId != User.Identity.Name)
            {
                // Trying to modify a record that does not belong to the user
                return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }

            _todoListRepository.Edit(todoList);

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

        // POST api/TodoList
        [ValidateHttpAntiForgeryToken]
        public HttpResponseMessage PostTodoList(TodoListDto todoListDto)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            todoListDto.UserId = User.Identity.Name;
            TodoList todoList = todoListDto.ToEntity();
            _todoListRepository.Add(todoList);
            _uow.Save();
            todoListDto.TodoListId = todoList.TodoListId;

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, todoListDto);
            response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = todoListDto.TodoListId }));
            return response;
        }

        // DELETE api/TodoList/5
        [ValidateHttpAntiForgeryToken]
        public HttpResponseMessage DeleteTodoList(int id)
        {
            TodoList todoList = _todoListRepository.Find(id);
            if (todoList == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            if (todoList.UserId != User.Identity.Name)
            {
                // Trying to delete a record that does not belong to the user
                return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }

            TodoListDto todoListDto = new TodoListDto(todoList);
            _todoListRepository.Delete(todoList);

            try
            {
                _uow.Save();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            return Request.CreateResponse(HttpStatusCode.OK, todoListDto);
        }

        protected override void Dispose(bool disposing)
        {
            _uow.Dispose();
            base.Dispose(disposing);
        }
    }
}