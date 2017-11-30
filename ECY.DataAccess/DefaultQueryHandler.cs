using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECY.DataAccess.Interfaces;

namespace ECY.DataAccess
{
    public class DefaultQueryHandler : IQueryHandler
    {
        private ISession _session;

        public DefaultQueryHandler(ISession session)
        {
            _session = session;
        }

        public T Query<T>(IQuery<T> query)
        {
            var result = query.Execute(_session);
            return result;
        }
    }
}
