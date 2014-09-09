using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcApplication2.Controllers;
using MvcApplication2.DA;
using MvcApplication2.Models;
using System.Collections.Generic;
using Moq;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;
using FluentAssertions;
using System.Linq;
using System.Threading;
using System.Security.Principal;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Net.Http;
using System.Web.Http.Hosting;
using System.Net;
using System.Web.Http.Routing;

namespace MvcApplication2.Tests
{
    [TestClass]
    public class TodoListController_Tests
    {
        TodoListController _controller;
        Mock<IRepository<TodoList>> _repository;
        Mock<IUnitOfWork> _uow;
        List<TodoList> _list = new List<TodoList>();
        const string _userId = "User";

        [TestInitialize]
        public void Initialize()
        {
            _repository = new Mock<IRepository<TodoList>>();
            _uow = new Mock<IUnitOfWork>();
            _uow.SetupGet(x => x.TodoListRepository).Returns(_repository.Object);
            _controller = new TodoListController(_uow.Object);
            _controller.Request = new HttpRequestMessage();
            _controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
            Thread.CurrentPrincipal = new GenericPrincipal
            (new GenericIdentity(_userId, "Forms"), new string[] { });
        }

        [TestMethod]
        public void GetAll_should_return_expected_todoLists()
        {
            _list.Add(new TodoList
            {
                Title = "Title1",
                UserId = _userId,
                TodoListId = 1,
                Todos = new List<TodoItem> 
                {
                    new TodoItem { IsDone = true, Priority = 1, TodoListId = 1, Title = "ITEM Title", Deadline = new DateTime(2014, 8, 31)},
                    new TodoItem { Title = "Write unit tests", TodoListId = 1, IsDone = true, Priority = 0, Deadline = new DateTime(2014, 8, 17) }
                }
            });
            _list.Add(new TodoList { Title = "Title2", UserId = _userId, TodoListId = 2, Todos = new List<TodoItem>() });
            _repository.Setup(x => x.GetAll(It.IsAny<string>())).Returns(_list.AsQueryable());

            var result = _controller.GetTodoLists();

            result.ShouldBeEquivalentTo(_list.OrderByDescending(l => l.TodoListId));
        }

        [TestMethod]
        public void GetAll_should_not_return_todoLists_with_other_userId()
        {
            _list.Add(new TodoList { Title = "Title1", UserId = _userId, TodoListId = 1, Todos = new List<TodoItem>() });
            _list.Add(new TodoList { Title = "Title2", UserId = "Another User", TodoListId = 2, Todos = new List<TodoItem>() });
            _repository.Setup(x => x.GetAll(It.IsAny<string>())).Returns(_list.AsQueryable());

            var result = _controller.GetTodoLists();

            result.ShouldBeEquivalentTo(new[] { new TodoList { Title = "Title1", UserId = _userId, TodoListId = 1, Todos = new List<TodoItem>() } });
        }

        [TestMethod]
        public void GetTodoList_should_return_todoList_by_id()
        {
            var expected = new TodoList { Title = "Title1", UserId = _userId, TodoListId = 1, Todos = new List<TodoItem>() };
            _list.Add(expected);
            _list.Add(new TodoList { Title = "Title2", UserId = _userId, TodoListId = 2, Todos = new List<TodoItem>() });
            _repository.Setup(x => x.Find(1)).Returns(expected);

            var todoList = _controller.GetTodoList(1);

            todoList.ShouldBeEquivalentTo(expected);
        }

        [TestMethod]
        public void GetTodoList_should_throw_HttpResponseException_if_userId_is_wrong()
        {
            var expected = new TodoList { Title = "Title1", UserId = "Another", TodoListId = 1, Todos = new List<TodoItem>() };
            _list.Add(expected);
            _list.Add(new TodoList { Title = "Title2", UserId = _userId, TodoListId = 2, Todos = new List<TodoItem>() });
            _repository.Setup(x => x.Find(1)).Returns(expected);

            Action action = () => _controller.GetTodoList(1);

            action.ShouldThrow<HttpResponseException>();
        }

        [TestMethod]
        public void PutTodoList_should_call_Edit_on_repository_and_Save_on_uow_and_return_message_with_status_ok_if_todoList_found()
        {
            int id = 1;
            _repository.Setup(x => x.Find(It.IsAny<TodoList>())).Returns(new TodoList { UserId = _userId, Todos = new List<TodoItem>() });
            
            var response = _controller.PutTodoList(id, new TodoListDto { TodoListId = id, Todos = new List<TodoItemDto>() });

            _repository.Verify(r => r.Edit(It.IsAny<TodoList>()), Times.Once);
            _uow.Verify(u => u.Save(), Times.Once);
            response.StatusCode.ShouldBeEquivalentTo(HttpStatusCode.OK);
        }

        [TestMethod]
        public void PutTodoList_should_not_call_Edit_on_repository_and_Save_on_uow_and_return_message_with_status_unathorized_if_todoList_userId_do_not_match()
        {
            int id = 1;
            _repository.Setup(x => x.Find(It.IsAny<TodoList>())).Returns(new TodoList { UserId = "Another", Todos = new List<TodoItem>() });

            var response = _controller.PutTodoList(id, new TodoListDto { TodoListId = id, Todos = new List<TodoItemDto>() });

            _repository.Verify(r => r.Edit(It.IsAny<TodoList>()), Times.Never);
            _uow.Verify(u => u.Save(), Times.Never);
            response.StatusCode.ShouldBeEquivalentTo(HttpStatusCode.Unauthorized);
        }

        [TestMethod]
        public void PostTodoList_should_call_Add_on_repository_and_Save_on_uow_and_return_message_with_status_created()
        {
            _controller.Configuration = new HttpConfiguration();
            var route = _controller.Configuration.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            var routeData = new HttpRouteData(route,
                   new HttpRouteValueDictionary 
            { 
                { "id", "1" },
                { "controller", "Users" } 
            }
            );
            _controller.Request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:9091/");
            _controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, _controller.Configuration);
            _controller.Request.Properties.Add(HttpPropertyKeys.HttpRouteDataKey, routeData);
 

            var response = _controller.PostTodoList(new TodoListDto { Todos = new List<TodoItemDto>() });

            _repository.Verify(r => r.Add(It.IsAny<TodoList>()), Times.Once);
            _uow.Verify(u => u.Save(), Times.Once);
            response.StatusCode.ShouldBeEquivalentTo(HttpStatusCode.Created);
        }

        [TestMethod]
        public void DeleteTodoList_should_call_Delete_on_repository_and_Save_on_uow_and_return_message_with_status_ok()
        {
            int id = 1;
            _repository.Setup(x => x.Find(id)).Returns(new TodoList { UserId = _userId, Todos = new List<TodoItem>() });

            var response = _controller.DeleteTodoList(id);

            _repository.Verify(r => r.Delete(It.IsAny<TodoList>()), Times.Once);
            _uow.Verify(u => u.Save(), Times.Once);
            response.StatusCode.ShouldBeEquivalentTo(HttpStatusCode.OK);
        }

        [TestMethod]
        public void DeleteTodoList_should_not_call_Edit_on_repository_and_Save_on_uow_and_return_message_with_status_unathorized_if_todoList_userId_do_not_match()
        {
            int id = 1;
            _repository.Setup(x => x.Find(id)).Returns(new TodoList { UserId = "Another", Todos = new List<TodoItem>() });

            var response = _controller.DeleteTodoList(id);

            _repository.Verify(r => r.Delete(It.IsAny<TodoList>()), Times.Never);
            _uow.Verify(u => u.Save(), Times.Never);
            response.StatusCode.ShouldBeEquivalentTo(HttpStatusCode.Unauthorized);
        }
    }
}
