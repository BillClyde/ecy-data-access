using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECY.DataAccess.Interfaces;

namespace DataAccessExample
{
    public class DeleteAddressCommandHandler : ICommandHandler<DeleteAddress>
    {
        private ISession _session;

        public DeleteAddressCommandHandler(ISession session)
        {
            _session = session;
        }

        public object Execute(DeleteAddress command)
        {
            return command.Execute(_session);
        }
    }
}
