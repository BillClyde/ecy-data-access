using ECY.DataAccess;
using ECY.DataAccess.Interfaces;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(x => x.FullName.Contains("DataAccessExample")).FirstOrDefault();
            container.Register<ISession>(() => new Session("AppConnection"), Lifestyle.Scoped);
            container.Register(typeof(IQueryHandler<,>), new[] { assemblies });
            container.Register(typeof(ICommandHandler<>), new[] { assemblies });
            //container.Register<IAddressService, AddressService>();

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
                //var service = container.GetInstance<IAddressService>();
                var session = container.GetInstance<ISession>();
                var getHandler = container.GetInstance<IQueryHandler<GetAddresses,IEnumerable<Address>>>();
                var insertHandler = container.GetInstance<ICommandHandler<InsertAddress>>();

                insertHandler.Execute(new InsertAddress(new Address {
                    Address1 = "134 Main Street",
                    City = "Olympia",
                    State = "WA",
                    PostalCode = "98516"
                }));

                foreach (var item in getHandler.Query(new GetAddresses()))
                {
                    Console.WriteLine(item);
                }
                Console.WriteLine("Save Entry? (y/N)");
                var keyPressed = Console.ReadKey();
                var yesKey = new string[] { "Y", "y" };
                if (keyPressed.Key.ToString() == "Y")
                {
                    session.Save();
                }

                foreach (var item in getHandler.Query(new GetAddresses()))
                {
                    Console.WriteLine(item);
                }
            }

            Console.WriteLine("Done.");
            Console.ReadKey();
        }
    }
}
