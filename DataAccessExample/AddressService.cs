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
            _database.Execute(new DeleteAddress(Id));
        }

        public IEnumerable<Address> GetAddresses()
        {
            return _database.Query(new GetAddresses());
        }

        public void InsertAddress(Address address)
        {
            _database.Execute(new InsertAddress(address));
        }

        public void UpdateAddress(Address address)
        {
            _database.Execute(new UpdateAddress(address));
        }
    }
}
