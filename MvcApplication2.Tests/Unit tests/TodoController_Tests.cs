using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MvcApplication2.DA;
using MvcApplication2.Models;
using MvcApplication2.Controllers;
using System.Net.Http;
using System.Web.Http.Hosting;
using System.Web.Http;
using System.Security.Principal;
using FluentAssertions;
using System.Net;
using System.Threading;
using System.Collections.Generic;
using System.Web.Http.Routing;

namespace MvcApplication2.Tests
{
    [TestClass]
    public class TodoController_Tests
    {
        TodoController _controller;
        Mock<IRepository<TodoList>> _todoListRepository;
        Mock<IRepository<TodoItem>> _todoRepository;
        Mock<IUnitOfWork> _uow;
        const string _userId = "User";

        [TestInitialize]
        public void Initialize()
        {
            _todoListRepository = new Mock<IRepository<TodoList>>();
            _todoRepository = new Mock<IRepository<TodoItem>>();
            _uow = new Mock<IUnitOfWork>();
            _uow.SetupGet(x => x.TodoListRepository).Returns(_todoListRepository.Object);
            _uow.SetupGet(x => x.TodoItemRepository).Returns(_todoRepository.Object);
            _controller = new TodoController(_uow.Object);
            _controller.Request = new HttpRequestMessage();
            _controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
            Thread.CurrentPrincipal = new GenericPrincipal
            (new GenericIdentity(_userId, "Forms"), new string[] { });
        }

        [TestMethod]
        public void PutTodoItem_should_call_Edit_and_Save_and_return_message_with_status_ok()
        {
            int listId = 1;
            int todoId = 11;
            _todoListRepository.Setup(x => x.Find(listId)).Returns(new TodoList { UserId = _userId, Todos = new List<TodoItem>() });

            var response = _controller.PutTodoItem(todoId, new TodoItemDto { TodoItemId = todoId, TodoListId = listId });

            _todoRepository.Verify(r => r.Edit(It.IsAny<TodoItem>()), Times.Once);
            _uow.Verify(u => u.Save(), Times.Once);
            response.StatusCode.ShouldBeEquivalentTo(HttpStatusCode.OK);
        }

        [TestMethod]
        public void PutTodoItem_should_not_call_Edit_and_Save_and_should_return_message_with_status_unathorized_if_todoList_userId_do_not_match()
        {
            int listId = 1;
            int todoId = 11;
            _todoListRepository.Setup(x => x.Find(listId)).Returns(new TodoList { UserId = "Another", Todos = new List<TodoItem>() });

            var response = _controller.PutTodoItem(todoId, new TodoItemDto { TodoItemId = todoId, TodoListId = listId });

            _todoRepository.Verify(r => r.Edit(It.IsAny<TodoItem>()), Times.Never);
            _uow.Verify(u => u.Save(), Times.Never);
            response.StatusCode.ShouldBeEquivalentTo(HttpStatusCode.Unauthorized);
        }

        [TestMethod]
        public void PutTodoItem_should_not_call_Edit_and_Save_and_should_return_message_with_status_not_found_if_todoList_not_found()
        {
            int listId = 1;
            int todoId = 11;
            _todoListRepository.Setup(x => x.Find(listId)).Returns<TodoList>(null);

            var response = _controller.PutTodoItem(todoId, new TodoItemDto { TodoItemId = todoId, TodoListId = listId });

            _todoRepository.Verify(r => r.Edit(It.IsAny<TodoItem>()), Times.Never);
            _uow.Verify(u => u.Save(), Times.Never);
            response.StatusCode.ShouldBeEquivalentTo(HttpStatusCode.NotFound);
        }

        [TestMethod]
        public void PostTodoItem_should_call_Add_on_repository_and_Save_on_uow_and_return_message_with_status_created()
        {
            InitializePostTest();
            int listId = 1;
            _todoListRepository.Setup(x => x.Find(listId)).Returns(new TodoList { UserId = _userId, Todos = new List<TodoItem>() });

            var response = _controller.PostTodoItem(new TodoItemDto { TodoListId = listId });

            _todoRepository.Verify(r => r.Add(It.IsAny<TodoItem>()), Times.Once);
            _uow.Verify(u => u.Save(), Times.Once);
            response.StatusCode.ShouldBeEquivalentTo(HttpStatusCode.Created);
        }

        [TestMethod]
        public void PostTodoItem_should_not_call_Add_on_repository_and_Save_on_uow_and_should_return_message_with_status_unauthorized_if_userId_not_match()
        {
            InitializePostTest();
            int listId = 1;
            _todoListRepository.Setup(x => x.Find(listId)).Returns(new TodoList { UserId = "AnotherId", Todos = new List<TodoItem>() });

            var response = _controller.PostTodoItem(new TodoItemDto { TodoListId = listId });

            _todoRepository.Verify(r => r.Add(It.IsAny<TodoItem>()), Times.Never);
            _uow.Verify(u => u.Save(), Times.Never);
            response.StatusCode.ShouldBeEquivalentTo(HttpStatusCode.Unauthorized);
        }

        [TestMethod]
        public void PostTodoItem_should_not_call_Add_on_repository_and_Save_on_uow_and_should_return_message_with_status_not_found_if_todoList_not_found()
        {
            InitializePostTest();
            int listId = 1;
            _todoListRepository.Setup(x => x.Find(listId)).Returns<TodoList>(null);

            var response = _controller.PostTodoItem(new TodoItemDto { TodoListId = listId });

            _todoRepository.Verify(r => r.Add(It.IsAny<TodoItem>()), Times.Never);
            _uow.Verify(u => u.Save(), Times.Never);
            response.StatusCode.ShouldBeEquivalentTo(HttpStatusCode.NotFound);
        }

        [TestMethod]
        public void DeleteTodoItem_should_call_Delete_on_repository_and_Save_on_uow_and_return_message_with_status_ok()
        {
            int id = 1;
            _todoRepository.Setup(x => x.Find(id)).Returns(new TodoItem { TodoItemId = id, TodoList = new TodoList { UserId = _userId } });

            var response = _controller.DeleteTodoItem(id);

            _todoRepository.Verify(r => r.Delete(It.IsAny<TodoItem>()), Times.Once);
            _uow.Verify(u => u.Save(), Times.Once);
            response.StatusCode.ShouldBeEquivalentTo(HttpStatusCode.OK);
        }

        [TestMethod]
        public void DeleteTodoItem_should_not_call_Delete_on_repository_and_Save_on_uow_and_return_message_with_status_unauthorized_if_userId_not_match()
        {
            int id = 1;
            _todoRepository.Setup(x => x.Find(id)).Returns(new TodoItem { TodoItemId = id, TodoList = new TodoList { UserId = "Another" } });

            var response = _controller.DeleteTodoItem(id);

            _todoRepository.Verify(r => r.Delete(It.IsAny<TodoItem>()), Times.Never);
            _uow.Verify(u => u.Save(), Times.Never);
            response.StatusCode.ShouldBeEquivalentTo(HttpStatusCode.Unauthorized);
        }

        [TestMethod]
        public void DeleteTodoItem_should_not_call_Delete_on_repository_and_Save_on_uow_and_return_message_with_status_not_found_if_taskItem_not_found()
        {
            int id = 1;
            _todoRepository.Setup(x => x.Find(id)).Returns<TodoItem>(null);

            var response = _controller.DeleteTodoItem(id);

            _todoRepository.Verify(r => r.Delete(It.IsAny<TodoItem>()), Times.Never);
            _uow.Verify(u => u.Save(), Times.Never);
            response.StatusCode.ShouldBeEquivalentTo(HttpStatusCode.NotFound);
        }

        private void InitializePostTest()
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
        }
    }
}
