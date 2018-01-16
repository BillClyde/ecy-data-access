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
        //void UpdateAddress(Address address);
        int InsertAddress(Address address);
        //void DeleteAddress(int Id);
        void Save();
    }
    public class AddressService : IAddressService
    {
        private ISession _session;
        private IQueryHandler<GetAddresses, IEnumerable<Address>> _getAddressHandler;
        private ICommandHandler<InsertAddress> _insertAddressHandler;

        public AddressService(ISession session, IQueryHandler<GetAddresses, IEnumerable<Address>> getAddressHandler, ICommandHandler<InsertAddress> insertAddressCommandHandler)
        {
            _session = session;
            _getAddressHandler = getAddressHandler;
            _insertAddressHandler = insertAddressCommandHandler;
        }

        //public void DeleteAddress(int Id)
        //{
        //    _database.Execute(new DeleteAddress(Id));
        //}

        public IEnumerable<Address> GetAddresses()
        {
            return _getAddressHandler.Query(new GetAddresses());
        }

        public int InsertAddress(Address address)
        {
            return (int)_insertAddressHandler.Execute(new InsertAddress(address));
        }

        public void Save()
        {
            _session.Save();
        }

        //public void UpdateAddress(Address address)
        //{
        //    _database.Execute(new UpdateAddress(address));
        //}
    }
}
