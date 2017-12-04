using ECY.DataAccess.Interfaces;

namespace DataAccessExample
{
    public class UpdateAddressCommandHandler : ICommandHandler<UpdateAddress>
    {
        private ISession _session;

        public UpdateAddressCommandHandler(ISession session)
        {
            _session = session;
        }
        public object Execute(UpdateAddress command)
        {
            return command.Execute(_session);
        }
    }
}
