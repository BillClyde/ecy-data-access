using ECY.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessExample
{
    public interface IAddressService
    {
        IEnumerable<Address> GetAddresses();
        void UpdateAddress(Address address);
        void InsertAddress(Address address);
        void DeleteAddress(int Id);
    }
    public class AddressService : IAddressService
    {
        private readonly IDatabase _database;

        public AddressService(IDatabase database)
        {
            _database = database;
        }

        public void DeleteAddress(int Id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Address> GetAddresses()
        {
            throw new NotImplementedException();
        }

        public void InsertAddress(Address address)
        {
            throw new NotImplementedException();
        }

        public void UpdateAddress(Address address)
        {
            throw new NotImplementedException();
        }
    }
}
