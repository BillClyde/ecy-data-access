using ECY.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessExample
{
    public class UpdateAddress : ICommand
    {
        Address _updatedAddress;

        public UpdateAddress(Address updatedAddress)
        {
            _updatedAddress = updatedAddress;
        }

        public object Execute(ISession session)
        {
            return session.Execute("spInsertAddress", new
            {
                Id = _updatedAddress.Id,
                Address1 = _updatedAddress.Address1,
                Address2 = _updatedAddress.Address2,
                City = _updatedAddress.City,
                State = _updatedAddress.State,
                PostalCode = _updatedAddress.PostalCode
            });
        }
    }
}
