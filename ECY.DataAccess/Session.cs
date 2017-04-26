using ECY.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace ECY.DataAccess
{
    public class Session : ISession, IDisposable
    {
        private readonly DataContext _context;
        private readonly UnitOfWork _unitOfWork;
        private bool disposed;

        public Session(string connectionStringName)
        {
            _context = new DataContext(connectionStringName);
            _unitOfWork = _context.CreateUnitOfWork();
        }

        public virtual IEnumerable<T> Query<T>(string query, object param = null, CommandType commandType = CommandType.StoredProcedure) where T : class, IEntity<T>, new()
        {
            return _context.Query<T>(query, param, commandType);
        }

        public virtual DataTable Query(string query, object param = null, CommandType commandType = CommandType.StoredProcedure)
        {
            return _context.Query(query, param, commandType);
        }

        public object Execute(string sql, object param = null)
        {
            return _context.Execute(sql, param);
        }

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
            if (!disposed)
            {
                _unitOfWork.Save();
                if (!disposing)
                {
                    _unitOfWork.Dispose();
                    _context.Dispose();
                }
            }

            disposed = true;
        }
    }
}
