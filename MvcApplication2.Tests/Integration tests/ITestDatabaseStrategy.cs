using MvcApplication2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcApplication2.Tests.Integration_tests
{
    public interface ITestDatabaseStrategy : IDisposable
    {
        TodoItemContext CreateContext();

        void Dispose(TodoItemContext context);
    }
}
