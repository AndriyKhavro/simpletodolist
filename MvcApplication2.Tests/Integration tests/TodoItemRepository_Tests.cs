using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcApplication2.DA;
using MvcApplication2.Models;
using System.Linq;
using FluentAssertions;

namespace MvcApplication2.Tests.Integration_tests
{
    [TestClass]
    public class TodoItemRepository_Tests : IntegrationTestBase
    {
        private IRepository<TodoItem> _todoItemRepository;

        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();
            _todoItemRepository = _uow.TodoItemRepository;
        }

        [TestMethod]
        public void Add_should_add_entity_if_todoList_exists()
        {
            var todoItem = new TodoItem { Title = "Task # 1", TodoListId = 1 };
            int oldCount = TestData.TodoItems.Count;

            _todoItemRepository.Add(todoItem);
            _uow.Save();
            UpdateRepository();

            _todoItemRepository.GetAll().Count().ShouldBeEquivalentTo(oldCount + 1);
        }

        [TestMethod]
        public void Add_should_throw_exception_if_todoList_does_not_exist()
        {
            var todoItem = new TodoItem { Title = "Task # 1", TodoListId = 11 };
            int oldCount = TestData.TodoItems.Count;

            Action action = () => _todoItemRepository.Add(todoItem);

            action.ShouldThrow<Exception>();
        }

        [TestMethod]
        public void Edit_should_change_passed_item()
        {
            string title = "New Title";
            var todoItem = TestData.TodoItems[0];
            todoItem.Title = title;

            _todoItemRepository.Edit(todoItem);
            _uow.Save();
            UpdateRepository();

            _todoItemRepository.Find(todoItem.TodoItemId).Title.ShouldBeEquivalentTo(title);
        }

        [TestMethod]
        public void Delete_should_delete_item()
        {
            var todoItem = _todoItemRepository.Find(1);

            _todoItemRepository.Delete(todoItem);
            _uow.Save();
            UpdateRepository();

            _todoItemRepository.GetAll().Count().ShouldBeEquivalentTo(TestData.TodoItems.Count - 1);
            _todoItemRepository.Find(1).Should().BeNull();
        }

        [TestMethod]
        public void Delete_by_id_should_delete_item()
        {
            _todoItemRepository.Delete(1);
            _uow.Save();
            UpdateRepository();

            _todoItemRepository.GetAll().Count().ShouldBeEquivalentTo(TestData.TodoItems.Count - 1);
            _todoItemRepository.Find(1).Should().BeNull();
        }

        private void UpdateRepository()
        {
            _uow.Dispose();
            _uow = new UnitOfWork(_testDatabaseStrategy.CreateContext());
            _todoItemRepository = _uow.TodoItemRepository;
        }
    }
}
