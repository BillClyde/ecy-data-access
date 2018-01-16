using System;
using System.Data;

namespace ECY.DataAccess
{
    /// <summary>
    /// Unit of work manager
    /// </summary>
    public class UnitOfWork : IDisposable
    {
        private IDbTransaction _transaction;
        private readonly Action<UnitOfWork> _onCommit;
        private readonly Action<UnitOfWork> _onRollback;

        /// <summary>
        /// Sets up the unit of work
        /// </summary>
        /// <param name="transaction">Transaction that wraps the unit of work</param>
        /// <param name="onCommitOrRollback">Action to take on commit or rollback</param>
        public UnitOfWork(IDbTransaction transaction, Action<UnitOfWork> onCommitOrRollback) : this(transaction, onCommitOrRollback, onCommitOrRollback)
        {
        }

        /// <summary>
        /// Sets up the unit of work
        /// </summary>
        /// <param name="transaction">Transaction that wraps the unit of work</param>
        /// <param name="onCommit">Action to take on commit</param>
        /// <param name="onRollback">Action to take on rollback</param>
        public UnitOfWork(IDbTransaction transaction, Action<UnitOfWork> onCommit, Action<UnitOfWork> onRollback)
        {
            _transaction = transaction;
            _onCommit = onCommit;
            _onRollback = onRollback;
        }

        /// <summary>
        /// Transaction that the unit of work has been wrapped in.
        /// </summary>
        public IDbTransaction Transaction
        {
            get { return _transaction; }
        }

        /// <summary>
        /// Save the unit of work
        /// </summary>
        public void Save()
        {
            if (_transaction == null) throw new InvalidOperationException("This unit of work has already been saved or undone");
            try
            {
                _transaction.Commit();
                _onCommit(this);
            }
            finally
            {
                _transaction.Dispose();
                _transaction = null;
            }
        }
        /// <summary>
        /// Dispose of the unit of work
        /// </summary>
        public void Dispose()
        {
            if (_transaction == null) return;
            try
            {
                _transaction.Rollback();
                _onRollback(this);
            }
            finally
            {
                _transaction.Dispose();
                _transaction = null;
            }
        }
    }
}
