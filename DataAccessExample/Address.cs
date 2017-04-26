using ECY.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace DataAccessExample
{
    public class Address : IEntity<Address>
    {
        public int Id { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }

        public IEnumerable<Address> Mapper(DataTable table)
        {
            var l = new List<Address>();
            foreach (DataRow row in table.Rows)
            {
                l.Add(new Address {
                    Id = row.Field<int>("Id"),
                    Address1 = row.Field<string>("Address1"),
                    Address2 = row.Field<string>("Address2"),
                    City = row.Field<string>("City"),
                    State = row.Field<string>("State"),
                    PostalCode = row.Field<string>("PostalCode")
                });
            }

            return l;
        }
    }
}
