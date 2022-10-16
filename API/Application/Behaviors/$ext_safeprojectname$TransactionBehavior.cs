using Cloud.$ext_safeprojectname$.Infrastructure.Base;
using Cloud.$ext_safeprojectname$.Infrastructure.DataContexts;
using Microsoft.Extensions.Logging;

namespace Cloud.$ext_safeprojectname$.API.Application.Behaviors
{
    /// <summary>
    /// Wraps all commands via "mediatR" library in a "transaction" block. Allows for "Transactions" across aggregates. 
    /// The core logic is inside BaseTransactionBehavior. The "Handle" function can be "overridden" to give clients
    /// flexibility for their own implementation.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public class $ext_safeprojectname$TransactionBehavior<TRequest, TResponse> : BaseTransactionBehavior<TRequest, TResponse, $ext_safeprojectname$TransactionBehavior<TRequest, TResponse>>  where TRequest: MediatR.IRequest<TResponse>
    {        
        public $ext_safeprojectname$TransactionBehavior($ext_safeprojectname$DataContext dbContext,
            ILogger<$ext_safeprojectname$TransactionBehavior<TRequest, TResponse>> logger) : base (dbContext, logger)
        {            
        }
    }
}
