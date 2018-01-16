using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
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
            object output = null;
            Dictionary<string, object> dict = new Dictionary<string, object> {
                { "Address1" , _address.Address1 },
                { "Address2" , _address.Address2 },
                { "City" , _address.City },
                { "State" , _address.State },
                { "PostalCode" , _address.PostalCode },
                { "Id", ParameterDirection.Output }
            };
            session.Execute("spInsertAddress", execute: cmd => {
                cmd.ExecuteNonQuery();
                output = ((IDbDataParameter)cmd.Parameters["@Id"]).Value;
            }, parseInputParams: cmd =>
            {
                foreach(var kvp in dict)
                {
                    var p = cmd.CreateParameter();
                    p.ParameterName = "@" + kvp.Key;
                    if(kvp.Value != null && kvp.Value.GetType() == typeof(ParameterDirection) && (ParameterDirection)kvp.Value == ParameterDirection.Output)
                    {
                        p.Direction = ParameterDirection.Output;
                        p.DbType = DbType.Int32;
                    }
                    else
                    {
                        p.Value = kvp.Value ?? DBNull.Value;
                    }
                    cmd.Parameters.Add(p);
                }
            });
            return output;
        }
    }
}
