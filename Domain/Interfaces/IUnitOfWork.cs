using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
namespace Cloud.$ext_safeprojectname$.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
        Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// Clients can set it to "false" to force the "TransactionBehavior<>" (If injected in "MediatorModule") class logic to activated selectively.
        /// </summary>
        bool AreGlobalTransactionsDisabled { get; set; }
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task CommitTransactionAsync(IDbContextTransaction transaction);
        void RollbackTransaction();
        /// <summary>
        /// Clients can supply a "custom" connection object for use in DBContexts. Useful in scenarios where a "transaction" may span multiple Contexts
        /// </summary>
        /// <param name="connection"></param>
        void ChangeConnectionInstance(DbConnection connection);
    }
}
