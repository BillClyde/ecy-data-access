using ECY.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using NLog;

namespace ECY.DataAccess
{
    public class DataContext : IDisposable
    {
        private readonly Logger log = LogManager.GetCurrentClassLogger();
        private IDbConnection _connection;
        private readonly DbConnectionFactory _connectionFactory;
        private readonly ReaderWriterLockSlim _rwLock = new ReaderWriterLockSlim();
        private readonly LinkedList<UnitOfWork> _workItems = new LinkedList<UnitOfWork>();
        public DataContext(string connectionStringName)
        {
            _connectionFactory = new DbConnectionFactory(connectionStringName);
        }

        public UnitOfWork CreateUnitOfWork(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            CreateOrReuseConnection();

            bool wasClosed = _connection.State == ConnectionState.Closed;
            if (wasClosed)
            {
                log.Debug("Opening db connection which is {0}", _connection.State == ConnectionState.Closed ? "Closed" : "Open");
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

        public IEnumerable<T> Query<T>(string sp, object param, CommandType commandType, Action<IDbCommand> parseInputParams = null) where T : class, IEntity<T>, new()
        {
            DataTable table = Query(sp, param, commandType, parseInputParams);
            return new T().Mapper(table);
        }

        public DataTable Query(string sp, object param, CommandType commandType, Action<IDbCommand> parseInputParams = null)
        {
            DataTable table;
            CreateOrReuseConnection();
            bool wasClosed = _connection.State == ConnectionState.Closed;
            if (wasClosed)
            {
                log.Debug("Opening db connection which is {0}", _connection.State == ConnectionState.Closed ? "Closed" : "Open");
                _connection.Open();
            }
            using (IDbCommand cmd = _connection.CreateCommand())
            {
                try
                {
                    cmd.CommandText = sp;
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

        public object Execute(string sp, object param, Action<IDbCommand> parseInputParams = null) 
        {
            CreateOrReuseConnection();
            bool wasClosed = _connection.State == ConnectionState.Closed;
            if (wasClosed)
            {
                log.Debug("Opening db connection which is {0}", _connection.State == ConnectionState.Closed ? "Closed" : "Open");
                _connection.Open();
            }
            try
            {
                using (IDbCommand cmd = _connection.CreateCommand())
                {
                    cmd.CommandText = sp;
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
                log.Debug("Closing db connection which is {0}", _connection.State == ConnectionState.Closed ? "Closed" : "Open");
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
                    log.Debug("Closing db connection which is {0}", _connection.State == ConnectionState.Closed ? "Closed" : "Open");
                    _connection.Dispose();
                }
                _connection = null;
            }
        }
    }
}
