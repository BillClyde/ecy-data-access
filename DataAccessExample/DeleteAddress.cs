using ECY.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessExample
{
    public class DeleteAddress : ICommand
    {
        private int _id;

        public DeleteAddress(int id)
        {
            _id = id;
        }

        public object Execute(ISession session)
        {
            return session.Execute("spDeleteAddress", new { Id = _id });
        }
    }
}
