using System;
using System.Collections.Generic;
using System.Data;

namespace ECY.DataAccess.Interfaces
{
    public interface ISession : IDisposable
    {
        IEnumerable<T> Query<T>(string query, object param = null, CommandType commandType = CommandType.StoredProcedure, Action<IDbCommand> parseInputParams = null, int timeout = 15) where T : class, IEntity<T>, new();
        DataTable Query(string query, object param = null, CommandType commandType = CommandType.StoredProcedure, Action<IDbCommand> parseInputParams = null, int timeout = 15);
        void Save();
        object Execute(string query, object param = null, Action<IDbCommand> parseInputParams = null, int timeout = 15);
    }
}
