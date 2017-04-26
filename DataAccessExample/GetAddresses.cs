using ECY.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessExample
{
    public class GetAddresses : IQuery<IEnumerable<Address>>
    {
        public IEnumerable<Address> Execute(ISession session)
        {
            return session.Query<Address>("SELECT * FROM Address", commandType: System.Data.CommandType.Text);
        }
    }
}
