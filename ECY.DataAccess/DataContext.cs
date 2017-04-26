using ECY.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;

namespace ECY.DataAccess
{
    public class DataContext : IDisposable
    {
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
            if (wasClosed) _connection.Open();

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

        public IEnumerable<T> Query<T>(string sp, object param, CommandType commandType) where T : class, IEntity<T>, new()
        {
            DataTable table = Query(sp, param, commandType);
            return new T().Mapper(table);
        }

        public DataTable Query(string sp, object param, CommandType commandType)
        {
            DataTable table;
            CreateOrReuseConnection();
            using (IDbCommand cmd = _connection.CreateCommand())
            {
                cmd.CommandText = sp;
                AddInputParameters(cmd, param);
                cmd.CommandType = commandType;
                cmd.Transaction = GetCurrentTransaction();
                using (IDataReader reader = cmd.ExecuteReader())
                {
                    table = new DataTable();
                    table.Load(reader);
                    return table;
                }
            }
        }

        public object Execute(string sp, object param) 
        {
            CreateOrReuseConnection();
            using (IDbCommand cmd = _connection.CreateCommand())
            {
                cmd.CommandText = sp;
                AddInputParameters(cmd, param);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Transaction = GetCurrentTransaction();
                return cmd.ExecuteScalar();
            };
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

            _connection.Close();
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
                _connection.Dispose();
                _connection = null;
            }
        }
    }
}
