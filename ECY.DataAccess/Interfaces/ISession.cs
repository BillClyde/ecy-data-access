using System;
using System.Collections.Generic;
using System.Data;

namespace ECY.DataAccess.Interfaces
{
    /// <summary>
    /// Database session with unit of work
    /// </summary>
    public interface ISession : IDisposable
    {
        /// <summary>
        /// Initialize a new Database Context
        /// </summary>
        /// <param name="connectionName">Name of connection string</param>
        void New(string connectionName);
        /// <summary>
        /// Strongly typed database query
        /// </summary>
        /// <typeparam name="T">Return model of database results</typeparam>
        /// <param name="query">Query string(defaults to stored procedure)</param>
        /// <param name="param">Optional object of parameters</param>
        /// <param name="commandType">Type of command if other than stored procedure</param>
        /// <param name="parseInputParams">Optional parameter parsing callback</param>
        /// <param name="timeout">Query timeout</param>
        /// <returns>IEnumerable of query model</returns>
        IEnumerable<T> Query<T>(string query, object param = null, CommandType commandType = CommandType.StoredProcedure, Action<IDbCommand> parseInputParams = null, int timeout = 15) where T : class, IEntity<T>, new();
        /// <summary>
        /// Dynamically typed database query
        /// </summary>
        /// <param name="query">Query string(defaults to stored procedure)</param>
        /// <param name="param">Optional object of parameters</param>
        /// <param name="commandType">Type of command if other than stored procedure</param>
        /// <param name="parseInputParams">Optional parameter parsing callback</param>
        /// <param name="timeout">Query timeout</param>
        /// <returns>Datatable of query results</returns>
        DataTable Query(string query, object param = null, CommandType commandType = CommandType.StoredProcedure, Action<IDbCommand> parseInputParams = null, int timeout = 15);
        /// <summary>
        /// Save session
        /// </summary>
        void Save();
        /// <summary>
        /// Execute stored procedure
        /// </summary>
        /// <param name="query">Query string(defaults to stored procedure)</param>
        /// <param name="param">Optional object of parameters</param>
        /// <param name="parseInputParams">Optional parameter parsing callback</param>
        /// <param name="timeout">Query timeout</param>
        /// <returns></returns>
        object Execute(string query, object param = null, Action<IDbCommand> parseInputParams = null, int timeout = 15);
        /// <summary>
        /// Execute stored procedure with execution callback
        /// </summary>
        /// <param name="sp">Query string(defaults to stored procedure)</param>
        /// <param name="execute">command execution callback</param>
        /// <param name="param">Optional object of parameters</param>
        /// <param name="parseInputParams">Optional parameter parsing callback</param>
        /// <param name="timeout">Query timeout</param>
        void Execute(string sp, Action<IDbCommand> execute, object param = null, Action<IDbCommand> parseInputParams = null, int timeout = 15);
    }
}
