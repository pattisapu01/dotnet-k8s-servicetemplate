using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Cloud.$ext_safeprojectname$.Domain.Helpers
{
    public abstract class BaseDBContext : DbContext
    {
        protected IDbContextTransaction _currentTransaction;
        public async Task<IDbContextTransaction> GetCurrentTransactionAsync(bool createNewIfNullTransaction)
        {
            if (createNewIfNullTransaction && _currentTransaction == null)
                await BeginTransactionAsync();
            return _currentTransaction;
        }

        public bool HasActiveTransaction => _currentTransaction != null;
        /// <summary>
        /// Clients can set it to "false" to force the "TransactionBehavior<>" (If injected in "MediatorModule") class logic to activated selectively.
        /// </summary>
        public bool AreGlobalTransactionsDisabled { get; set; }
        public BaseDBContext(DbContextOptions options) : base(options)
        {

        }
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            if (_currentTransaction != null) return null;

            _currentTransaction = await Database.BeginTransactionAsync(CancellationToken.None);

            return _currentTransaction;
        }

        public async Task CommitTransactionAsync(IDbContextTransaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (transaction != _currentTransaction) throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

            try
            {
                await SaveChangesAsync();
                transaction.Commit();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public void RollbackTransaction()
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        /// <summary>
        /// Implementors have to override this method
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public abstract Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// TODO: Load the schema name based on customer context.
        /// </summary>
        protected string SchemaName { get => "default"; }
        public async void ChangeConnectionInstance(DbConnection connection)
        {
            var conn = this.Database.GetDbConnection();
            await conn.CloseAsync();
            this.Database.SetDbConnection(connection);
        }
    }

    public class Operations
    {
        public enum DatabaseOpType
        {
            Add = 1,
            Update = 2,
            Delete = 3
        }
    }
}