using ECY.DataAccess.Interfaces;

namespace ECY.DataAccess
{
    public class Database : IDatabase
    {
        private ISession _session { get; set; }

        public Database(ISession session)
        {
            _session = session;
        }

        public T Query<T>(IQuery<T> query)
        {
            var result = query.Execute(_session);
            return result;
        }

        public object Execute(ICommand command)
        {
            return command.Execute(_session);
        }
    }
}
