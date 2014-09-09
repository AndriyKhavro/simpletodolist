using MvcApplication2.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcApplication2.Tests.Integration_tests
{
    public class SqlCeDatabaseStrategy : ITestDatabaseStrategy
    {
        public SqlCeDatabaseStrategy() :
            this("RealMyAppDb")
        {
        }

        public SqlCeDatabaseStrategy(string databaseName)
        {
            DatabaseName = databaseName;
        }

        public string DatabaseName { get; private set; }

        public bool DatabaseInitialized { get; private set; }

        public TodoItemContext CreateContext()
        {
            // create the database from scratch if it has not been initialised
            if (!DatabaseInitialized)
            {
                // get the path to the .SDF file
                var fullPath = GetRealDatabaseFilePath(DatabaseName);

                // delete it if it already exists
                if (File.Exists(fullPath))
                    File.Delete(fullPath);

                // get the SQL CE connection string
                string connectionString = GetRealDatabaseConnectionString(DatabaseName);

                // NEED TO SET THIS TO MAKE DATABASE CREATION WORK WITH SQL CE!!!
               // Database.DefaultConnectionFactory = new SqlCeConnectionFactory("System.Data.SqlServerCe.4.0");

                using (var context = new TodoItemContext(connectionString))
                {
                    context.Database.Create();
                }

                // Set the initialised flag so that we don't re-create the database again for this instance of the class
                DatabaseInitialized = true;
            }

            // create the DbContext with the SQL CE connection string
            return new TodoItemContext(GetRealDatabaseConnectionString(DatabaseName));
        }

        public void Dispose(TodoItemContext context)
        {
            try
            {
                Dispose();
            }
            finally
            {
                if (context != null)
                {
                    context.Dispose();
                }
            }
        }

        public void Dispose()
        {
            // delete the database
            var fullPath = GetRealDatabaseFilePath(DatabaseName);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            DatabaseInitialized = false;
        }

        private static string GetRealDatabaseFilePath(string databaseName)
        {
            var directory = Path.GetTempPath(); 
            var fileName = String.Format("{0}.sdf", databaseName);

            return Path.Combine(directory, fileName);
        }

        private static string GetRealDatabaseConnectionString(string databaseName)
        {
            return string.Format("Data Source = {0}", GetRealDatabaseFilePath(databaseName));
        }
    }
}
