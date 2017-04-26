using ECY.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessExample
{
    public class InsertAddress : ICommand
    {
        private Address _address;
        public InsertAddress(Address address)
        {
            _address = address;
        }

        public object Execute(ISession session)
        {
            return session.Execute("spInsertAddress", new
            {
                Address1 = _address.Address1,
                Address2 = _address.Address2,
                City = _address.City,
                State = _address.State,
                PostalCode = _address.PostalCode
            });
        }
    }
}
