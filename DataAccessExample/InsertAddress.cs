using System;
using System.Collections.Generic;
using ECY.DataAccess.Interfaces;

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
            //return session.Execute("spInsertAddress", new
            //{
            //    Address1 = _address.Address1,
            //    Address2 = _address.Address2,
            //    City = _address.City,
            //    State = _address.State,
            //    PostalCode = _address.PostalCode
            //});
            Dictionary<string, object> dict = new Dictionary<string, object> {
                { "Address1" , _address.Address1 },
                { "Address2" , _address.Address2 },
                { "City" , _address.City },
                { "State" , _address.State },
                { "PostalCode" , _address.PostalCode }
            };
            return session.Execute("spInsertAddress", parseInputParams: cmd =>
            {
                foreach(var kvp in dict)
                {
                    var p = cmd.CreateParameter();
                    p.ParameterName = "@" + kvp.Key;
                    p.Value = kvp.Value ?? DBNull.Value;
                    cmd.Parameters.Add(p);
                }
            });
        }
    }
}
