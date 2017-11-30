using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECY.DataAccess.Interfaces;

namespace ECY.DataAccess
{
    class DefaultCommandHandler : ICommandHandler
    {
        private readonly ISession _session;

        public DefaultCommandHandler(ISession session)
        {
            _session = session;
        }

        public object Execute(ICommand command)
        {
            return command.Execute(_session);
        }
    }
}
