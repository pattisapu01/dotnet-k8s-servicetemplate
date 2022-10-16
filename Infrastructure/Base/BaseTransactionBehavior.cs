using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Serilog.Context;
using Microsoft.Extensions.Logging;
using Cloud.$ext_safeprojectname$.Domain.Helpers;
using Cloud.$ext_safeprojectname$.Infrastructure.Extensions;
namespace Cloud.$ext_safeprojectname$.Infrastructure.Base
{
    public abstract class BaseTransactionBehavior<TRequest, TResponse, TImplementation> : IPipelineBehavior<TRequest, TResponse> where TImplementation : BaseTransactionBehavior<TRequest, TResponse, TImplementation> where TRequest : IRequest<TResponse>
    {
        protected readonly ILogger<TImplementation> _logger;
        protected readonly BaseDBContext _dbContext;
        protected readonly bool _areGlobalTransactionsDisabled;
        //protected readonly IMeshEventService _meshEventService;
        public BaseTransactionBehavior(BaseDBContext dbContext, ILogger<TImplementation> logger)// IMeshEventService meshEventService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _areGlobalTransactionsDisabled = dbContext.AreGlobalTransactionsDisabled;
            //_meshEventService = meshEventService ?? throw new ArgumentException(nameof(meshEventService));
        }
        public async virtual Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {            
            var response = default(TResponse);
            var typeName = request.GetGenericTypeName();

            try
            {
                if (_areGlobalTransactionsDisabled) // || request is MeshEvent {AreTransactionsEnabled: false}) //don't invoke "DB transaction" behavior for "mesh" events if AreTransactionsEnabled: false
                    return await next();
                
                if (_dbContext.HasActiveTransaction)
                {
                    //the client started a transaction OR line # 50 (using (var transaction = await _dbContext.BeginTransactionAsync())) already executed
                    return await next();
                }

                var strategy = _dbContext.Database.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    Guid transactionId;

                    using (var transaction = await _dbContext.BeginTransactionAsync())
                    using (LogContext.PushProperty("TransactionContext", transaction.TransactionId))
                    {
                        _logger.LogDebug("----- Begin transaction {TransactionId} for {CommandName} ({@Command})", transaction.TransactionId, typeName, request);

                        response = await next();

                        _logger.LogDebug("----- Commit transaction {TransactionId} for {CommandName}", transaction.TransactionId, typeName);

                        await _dbContext.CommitTransactionAsync(transaction);

                        transactionId = transaction.TransactionId;
                    }

                    //await _meshEventService.PublishEventsThroughEventBusAsync(transactionId);
                });

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR Handling transaction for {CommandName} ({@Command})", typeName, request);

                throw;
            }
        }
    }
}
