using ECY.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using NLog;

namespace ECY.DataAccess
{
    /// <summary>
    /// Instantiates a Data Context and Unit of Work
    /// </summary>
    public class Session : ISession, IDisposable
    {
        private readonly Logger log = LogManager.GetCurrentClassLogger();
        private readonly DataContext _context;
        private readonly UnitOfWork _unitOfWork;
        private bool disposed;

        /// <summary>
        /// Main initializer for session
        /// </summary>
        /// <param name="connectionStringName">Name of database connection as sored in App.config or Web.config</param>
        public Session(string connectionStringName)
        {
            _context = new DataContext(connectionStringName);
            _unitOfWork = _context.CreateUnitOfWork();
        }

        /// <summary>
        /// Executes a stored procedure(default) or select statement and returns a result that is mapped to an IEntity class
        /// </summary>
        /// <typeparam name="T">POCO that is of type IEntity</typeparam>
        /// <param name="query">stored procedure or select statement</param>
        /// <param name="param">Object containing parameters that correspond to the SQL parameters</param>
        /// <param name="commandType">Defaults to stored procedure</param>
        /// <param name="parseInputParams">Alternate way of parsing SQL parameters if param object can't be used</param>
        /// <returns>Enumberable of type IEntity with the results of the query</returns>
        public virtual IEnumerable<T> Query<T>(string query, object param = null, CommandType commandType = CommandType.StoredProcedure, Action<IDbCommand> parseInputParams = null) where T : class, IEntity<T>, new()
        {
            return _context.Query<T>(query, param, commandType, parseInputParams);
        }

        /// <summary>
        /// Executes a stored procedure(default) or select statement and returns the result in a DataTable
        /// </summary>
        /// <param name="query">stored procedure or select statement</param>
        /// <param name="param">Object containing parameters that correspond to the SQL parameters</param>
        /// <param name="commandType">Defaults to stored procedure</param>
        /// <param name="parseInputParams">Alternate way of parsing SQL parameters if param object can't be used</param>
        /// <returns>DataTable with the results of the query</returns>
        public virtual DataTable Query(string query, object param = null, CommandType commandType = CommandType.StoredProcedure, Action<IDbCommand> parseInputParams = null)
        {
            return _context.Query(query, param, commandType, parseInputParams);
        }

        /// <summary>
        /// Executes an Insert/Update/Delete stored procedure and returns the result
        /// </summary>
        /// <param name="sql">Insert/Update/Delete stored procudure</param>
        /// <param name="param">Object containing parameters that correspond to the SQL parameters</param>
        /// <param name="parseInputParams">Alternate way of parsing SQL parameters if param object can't be used</param>
        /// <returns>Result of the stored proc</returns>
        public object Execute(string sql, object param = null, Action<IDbCommand> parseInputParams = null)
        {
            return _context.Execute(sql, param, parseInputParams);
        }

        /// <summary>
        /// Commits the data to the database
        /// </summary>
        public void Save()
        {
            _unitOfWork.Save();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            log.Debug("Disposing session with {0}", disposing);
            if (!disposed)
            {
                if (disposing)
                {
                    log.Debug("Disposing unitofwork and context");
                    _unitOfWork.Dispose();
                    _context.Dispose();
                }
            }

            disposed = true;
        }
    }
}
