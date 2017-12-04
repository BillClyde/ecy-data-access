using System.Collections.Generic;
using ECY.DataAccess.Interfaces;

namespace DataAccessExample
{
    public class GetAddressQueryHandler : IQueryHandler<GetAddresses, IEnumerable<Address>>
    {
        private ISession _session;

        public GetAddressQueryHandler(ISession session)
        {
            _session = session;
        }

        public IEnumerable<Address> Query(GetAddresses query)
        {
            return query.Execute(_session);
        }
    }
}
