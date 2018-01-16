using ECY.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using Common.Logging;

namespace ECY.DataAccess
{
    /// <summary>
    /// Data Context for queries
    /// </summary>
    public class DataContext : IDisposable
    {
        private readonly ILog log = LogManager.GetLogger<DataContext>();
        private IDbConnection _connection;
        private readonly DbConnectionFactory _connectionFactory;
        private readonly ReaderWriterLockSlim _rwLock = new ReaderWriterLockSlim();
        private readonly LinkedList<UnitOfWork> _workItems = new LinkedList<UnitOfWork>();

        /// <summary>
        /// Constructor for setting up the database connection
        /// </summary>
        /// <param name="connectionStringName">Name of the connection string</param>
        public DataContext(string connectionStringName)
        {
            _connectionFactory = new DbConnectionFactory(connectionStringName);
        }

        /// <summary>
        /// Unit of work initialization
        /// </summary>
        /// <param name="isolationLevel"></param>
        /// <returns></returns>
        public UnitOfWork CreateUnitOfWork(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            CreateOrReuseConnection();

            bool wasClosed = _connection.State == ConnectionState.Closed;
            if (wasClosed)
            {
                log.DebugFormat("Opening db connection which is {0}", _connection.State == ConnectionState.Closed ? "Closed" : "Open");
                _connection.Open();
            }

            try
            {
                UnitOfWork unit;
                IDbTransaction transaction = _connection.BeginTransaction(isolationLevel);
                if (wasClosed)
                    unit = new UnitOfWork(transaction, RemoveTransactionAndCloseConnection);
                else
                    unit = new UnitOfWork(transaction, RemoveTransaction);

                _rwLock.EnterWriteLock();
                _workItems.AddLast(unit);
                _rwLock.ExitWriteLock();

                return unit;
            }
            catch
            {
                if (wasClosed) _connection.Close();

                throw;
            }
        }

        /// <summary>
        /// Strongly typed Query
        /// </summary>
        /// <typeparam name="T">Model for query return</typeparam>
        /// <param name="sp">Stored Procedure for the query, optionally can be sql script</param>
        /// <param name="param">Object of parameters</param>
        /// <param name="commandType">Type of command (Stored Proc or SQL Script)</param>
        /// <param name="parseInputParams">Optional callback for parsing parameters</param>
        /// <param name="timeout">Query timeout</param>
        /// <returns>IEnumerable of query results</returns>
        public IEnumerable<T> Query<T>(string sp, object param, CommandType commandType, Action<IDbCommand> parseInputParams = null, int timeout = 15) where T : class, IEntity<T>, new()
        {
            DataTable table = Query(sp, param, commandType, parseInputParams, timeout);
            return new T().Mapper(table);
        }

        /// <summary>
        /// Dynamically typed Query
        /// </summary>
        /// <param name="sp">Stored Procedure for the query, optionally can be sql script</param>
        /// <param name="param">Object of parameters</param>
        /// <param name="commandType">Type of command (Stored Proc or SQL Script)</param>
        /// <param name="parseInputParams">Optional callback for parsing parameters</param>
        /// <param name="timeout">Query timeout</param>
        /// <returns>Datatable with query results</returns>
        public DataTable Query(string sp, object param, CommandType commandType, Action<IDbCommand> parseInputParams = null, int timeout = 15)
        {
            DataTable table;
            CreateOrReuseConnection();
            bool wasClosed = _connection.State == ConnectionState.Closed;
            if (wasClosed)
            {
                log.DebugFormat("Opening db connection which is {0}", _connection.State == ConnectionState.Closed ? "Closed" : "Open");
                _connection.Open();
            }
            using (IDbCommand cmd = _connection.CreateCommand())
            {
                try
                {
                    cmd.CommandText = sp;
                    cmd.CommandTimeout = timeout;
                    if(parseInputParams == null)
                    {
                        AddInputParameters(cmd, param);
                    }
                    else
                    {
                        parseInputParams(cmd);
                    }
                    cmd.CommandType = commandType;
                    cmd.Transaction = GetCurrentTransaction();
                    using (IDataReader reader = cmd.ExecuteReader())
                    {
                        table = new DataTable();
                        table.Load(reader);
                        return table;
                    }
                }
                finally
                {
                    if (wasClosed) _connection.Close();
                }
            }
        }

        /// <summary>
        /// Execute stored proc using ExecuteScalar 
        /// </summary>
        /// <param name="sp">Stored Procedure for the query, optionally can be sql script</param>
        /// <param name="param">Object of parameters</param>
        /// <param name="parseInputParams">Optional callback for parsing parameters</param>
        /// <param name="timeout">Query timeout</param>
        /// <returns></returns>
        public object Execute(string sp, object param, Action<IDbCommand> parseInputParams = null, int timeout = 15) 
        {
            CreateOrReuseConnection();
            bool wasClosed = _connection.State == ConnectionState.Closed;
            if (wasClosed)
            {
                log.DebugFormat("Opening db connection which is {0}", _connection.State == ConnectionState.Closed ? "Closed" : "Open");
                _connection.Open();
            }
            try
            {
                using (IDbCommand cmd = _connection.CreateCommand())
                {
                    cmd.CommandText = sp;
                    cmd.CommandTimeout = timeout;
                    if(parseInputParams == null)
                    {
                        AddInputParameters(cmd, param);
                    }
                    else
                    {
                        parseInputParams(cmd);
                    }
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Transaction = GetCurrentTransaction();
                    return cmd.ExecuteScalar();
                }

            }
            finally
            {
                if (wasClosed) _connection.Close();
            }
        }

        /// <summary>
        /// Execute stored procudure using callback
        /// </summary>
        /// <param name="sp">The stored procedure to execute</param>
        /// <param name="execute">Command execution callback</param>
        /// <param name="param">Object containing parameters</param>
        /// <param name="parseInputParams">Optional parameter parsing callback</param>
        /// <param name="timeout">Optional query timeout</param>
        public void Execute(string sp, Action<IDbCommand> execute, object param, Action<IDbCommand> parseInputParams = null, int timeout = 15)
        {
            CreateOrReuseConnection();
            bool wasClosed = _connection.State == ConnectionState.Closed;
            if (wasClosed)
            {
                log.DebugFormat("Opening db connection which is {0}", _connection.State == ConnectionState.Closed ? "Closed" : "Open");
                _connection.Open();
            }
            try
            {
                using (IDbCommand cmd = _connection.CreateCommand())
                {
                    cmd.CommandText = sp;
                    cmd.CommandTimeout = timeout;
                    if(parseInputParams == null)
                    {
                        AddInputParameters(cmd, param);
                    }
                    else
                    {
                        parseInputParams(cmd);
                    }
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Transaction = GetCurrentTransaction();
                    execute(cmd);
                }

            }
            finally
            {
                if (wasClosed) _connection.Close();
            }
        }

        private void AddInputParameters<T>(IDbCommand cmd, T parameters) where T : class
        {
            if (parameters != null)
            {
                foreach (var prop in parameters.GetType().GetProperties())
                {
                    object val = prop.GetValue(parameters, null);
                    var p = cmd.CreateParameter();
                    p.ParameterName = "@" + prop.Name;
                    if (val != null && val.GetType() == typeof(ParameterDirection) && (ParameterDirection)val == ParameterDirection.Output)
                    {
                        p.Direction = ParameterDirection.Output;
                        p.DbType = DbType.String;
                        p.Size = 4000;
                    }
                    else
                    {
                        p.Value = val ?? DBNull.Value;
                    }

                    cmd.Parameters.Add(p);
                }
            }
        }

        private void CreateOrReuseConnection()
        {
            if (_connection != null) return;
            _connection = _connectionFactory.Create();
        }

        private void RemoveTransaction(UnitOfWork workItem)
        {
            _rwLock.EnterWriteLock();
            _workItems.Remove(workItem);
            _rwLock.ExitWriteLock();
        }

        private void RemoveTransactionAndCloseConnection(UnitOfWork workItem)
        {
            _rwLock.EnterWriteLock();
            _workItems.Remove(workItem);
            _rwLock.ExitWriteLock();

            if (_connection.State == ConnectionState.Open)
            {
                log.DebugFormat("Closing db connection which is {0}", _connection.State == ConnectionState.Closed ? "Closed" : "Open");
                _connection.Close();
            }
        }

        private IDbTransaction GetCurrentTransaction()
        {
            IDbTransaction currentTransaction = null;
            _rwLock.EnterReadLock();
            if (_workItems.Any()) currentTransaction = _workItems.First.Value.Transaction;
            _rwLock.ExitReadLock();

            return currentTransaction;
        }

        /// <summary>
        /// Disose of context cleaning up any unit of work objects.
        /// </summary>
        public void Dispose()
        {
            log.Debug("Data context being disposed");
            _rwLock.EnterUpgradeableReadLock();
            try
            {
                while (_workItems.Any())
                {
                    var workItem = _workItems.First;
                    workItem.Value.Dispose();
                }
            }
            finally
            {
                _rwLock.ExitUpgradeableReadLock();
            }

            if(_connection != null)
            {
                if(_connection.State == ConnectionState.Open) {
                    log.DebugFormat("Closing db connection which is {0}", _connection.State == ConnectionState.Closed ? "Closed" : "Open");
                    _connection.Dispose();
                }
                _connection = null;
            }
        }
    }
}
