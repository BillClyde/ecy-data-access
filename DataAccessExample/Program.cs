using ECY.DataAccess;
using ECY.DataAccess.Interfaces;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using System;
using System.Collections.Generic;
using System.Data;

namespace DataAccessExample
{
    /// <summary>
    /// Demo of DataContext
    /// </summary>
    class Program
    {
        static readonly Container container;

        /// <summary>
        /// We are using Simple Injector for IoC here but any IoC container can be used. Or you can omit IoC.
        /// </summary>
        static Program()
        {
            // Set up the IoC container
            container = new Container();
            container.Options.DefaultScopedLifestyle = new ThreadScopedLifestyle();

            //Session needs to have the connection name passed to it.
            container.Register<ISession>(() => new Session("AppConnection"), Lifestyle.Scoped);
            container.Register<IDatabase, Database>();
            container.Register<IService, Service>();

            try
            {
                container.Verify();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        static void Main(string[] args)
        {
            using (ThreadScopedLifestyle.BeginScope(container))
            {
                // In web applications you wouldn't do this you would use the Controller's constructor to get the service instance
                // In either case the session automatically wraps the database actions in a transaction, any changes are saved
                // to the database when the scope ends.
                var service = container.GetInstance<IService>();

                foreach (var item in service.GetLogins())
                {
                    Console.WriteLine(item);
                }
            }

            Console.WriteLine("Done.");
            Console.ReadKey();
        }
    }

    public interface IService
    {
        IEnumerable<string> GetLogins();
    }

    /// <summary>
    /// Example of a service that calls the database stored procs
    /// </summary>
    public class Service : IService
    {
        private readonly IDatabase _database;

        /// <summary>
        /// This captures the injected instance variables.
        /// </summary>
        /// <param name="database">The injected database instance</param>
        public Service(IDatabase database)
        {
            _database = database;
        }

        /// <summary>
        /// Sample call to a database instance
        /// </summary>
        /// <returns>The results of the query</returns>
        public IEnumerable<string> GetLogins()
        {
            return _database.Query(new GetLogins());
        }
    }

    /// <summary>
    /// Example of a query using the CQRS pattern
    /// </summary>
    public class GetLogins : IQuery<IEnumerable<string>>
    {
        public IEnumerable<string> Execute(ISession session)
        {
            var list = new List<string>();
            var result = session.Query("spExtraLoginsGetLogins");
            foreach (DataRow row in result.Rows)
            {
                list.Add(row.Field<string>(1));
            }

            return list;
        }
    }
}
