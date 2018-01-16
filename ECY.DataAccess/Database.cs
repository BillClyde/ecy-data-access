using ECY.DataAccess.Interfaces;

namespace ECY.DataAccess
{
    /// <summary>
    /// Overall database interface. Deprecated, use ICommandHandler and IQueryHandler instead
    /// </summary>
    public class Database : IDatabase
    {
        private ISession _session { get; set; }

        /// <summary>
        /// Injectable constructor to initialize database with a session
        /// </summary>
        /// <param name="session"></param>
        public Database(ISession session)
        {
            _session = session;
        }

        /// <summary>
        /// Typed query
        /// </summary>
        /// <typeparam name="T">Query return model</typeparam>
        /// <param name="query">Query to execute</param>
        /// <returns></returns>
        public T Query<T>(IQuery<T> query)
        {
            var result = query.Execute(_session);
            return result;
        }

        /// <summary>
        /// Database command
        /// </summary>
        /// <param name="command">Command to execute</param>
        /// <returns></returns>
        public object Execute(ICommand command)
        {
            return command.Execute(_session);
        }
    }
}
