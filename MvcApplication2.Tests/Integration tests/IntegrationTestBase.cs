using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcApplication2.DA;

namespace MvcApplication2.Tests.Integration_tests
{
    [TestClass]
    public class IntegrationTestBase
    {
        protected UnitOfWork _uow;
        protected ITestDatabaseStrategy _testDatabaseStrategy;

        [TestInitialize]
        public virtual void Initialize()
        {
            _testDatabaseStrategy = new SqlCeDatabaseStrategy();
            using (var tempContext = _testDatabaseStrategy.CreateContext())
            {
                TestData.Add2TodoListsAnd3TodoItems(tempContext);
                tempContext.SaveChanges();
            }

            // initialise the repository we are testing
            var context = _testDatabaseStrategy.CreateContext();
            _uow = new UnitOfWork(context);
        }

        [TestCleanup]
        public virtual void Cleanup()
        {
            _testDatabaseStrategy.Dispose();
            _uow.Dispose();
        }
    }
}
