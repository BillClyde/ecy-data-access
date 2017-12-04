using ECY.DataAccess.Interfaces;

namespace DataAccessExample
{
    public class InsertAddressCommandHandler : ICommandHandler<InsertAddress>
    {
        private ISession _session;

        public InsertAddressCommandHandler(ISession session)
        {
            _session = session;
        }

        public object Execute(InsertAddress command)
        {
            return command.Execute(_session);
        }
    }
}
