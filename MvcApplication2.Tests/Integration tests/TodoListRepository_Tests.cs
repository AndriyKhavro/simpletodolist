using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcApplication2.Models;
using MvcApplication2.DA;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;

namespace MvcApplication2.Tests.Integration_tests
{
    [TestClass]
    public class TodoListRepository_Tests : IntegrationTestBase
    {
        private IRepository<TodoList> _todoListRepository;

        [TestInitialize]
        public override void Initialize()
        {
            base.Initialize();
            _todoListRepository = _uow.TodoListRepository;
        }

        [TestMethod]
        public void GetAll_should_return_all_todoLists()
        {
            var lists = _todoListRepository.GetAll();
            
            lists.ShouldBeEquivalentTo(TestData.TodoLists,
                options => options.Excluding(l => l.PropertyInfo.Name == "Todos"));
        }

        [TestMethod]
        public void Find_should_return_entity_by_id()
        {
            var list = _todoListRepository.Find(1);

            list.ShouldBeEquivalentTo(TestData.TodoLists.Single(x => x.TodoListId == 1),
                options => options.Excluding(l => l.PropertyInfo.Name == "Todos"));
        }

        [TestMethod]
        public void Find_should_return_null_if_list_does_not_exist()
        {
            var list = _todoListRepository.Find(11);

            list.ShouldBeEquivalentTo(null);
        }

        [TestMethod]
        public void Add_should_add_entity_to_db()
        {
            var listToAdd = new TodoList { UserId = "User", Title = "The new project" };

            _todoListRepository.Add(listToAdd);
            _uow.Save();

            _todoListRepository.GetAll().Count().ShouldBeEquivalentTo(TestData.TodoLists.Count + 1);
        }

        [TestMethod]
        public void Edit_should_change_title_of_todoList_if_list_exists()
        {
            int id = 1;
            string title = "New Title";
            var listToChange = TestData.TodoLists[0];
            listToChange.Title = title;

            _todoListRepository.Edit(listToChange);
            _uow.Save();

            _todoListRepository.Find(id).ShouldBeEquivalentTo(listToChange);
        }

        [TestMethod]
        public void Delete_should_delete_entity()
        {
            int id = 1;
            var listToDelete = _todoListRepository.Find(id);

            _todoListRepository.Delete(listToDelete);
            _uow.Save();

            _todoListRepository.GetAll().Count().ShouldBeEquivalentTo(TestData.TodoLists.Count - 1);
        }

        [TestMethod]
        public void Delete_should_delete_entity_by_id()
        {
            int id = 1;

            _todoListRepository.Delete(id);
            _uow.Save();

            _todoListRepository.GetAll().Count().ShouldBeEquivalentTo(TestData.TodoLists.Count - 1);
        }
    }
}
