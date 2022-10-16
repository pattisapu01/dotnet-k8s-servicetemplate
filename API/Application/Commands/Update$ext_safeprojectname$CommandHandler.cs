
using Cloud.$ext_safeprojectname$.Domain.Helpers;
using Cloud.$ext_safeprojectname$.Domain.Kernel;
using Cloud.$ext_safeprojectname$.Infrastructure.DataContexts;
using Cloud.$ext_safeprojectname$.Infrastructure.Repos;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloud.$ext_safeprojectname$.API.Application.Commands
{  
    public class Update$ext_safeprojectname$CommandHandler : IRequestHandler<Update$ext_safeprojectname$Command, Application.DTO.$ext_safeprojectname$>
    {
        private readonly I$ext_safeprojectname$Repo _$ext_safeprojectname$Repo;
        //private readonly IIdentityService _identityService; //TODO
        private readonly IMediator _mediator;
        //private readonly I$ext_safeprojectname$MetaDataEnterpriseEventService _$ext_safeprojectname$MetaDataEnterpriseEventService; //TODO
        private readonly ILogger<Update$ext_safeprojectname$CommandHandler> _logger;
        

        public Update$ext_safeprojectname$CommandHandler(IMediator mediator,
            I$ext_safeprojectname$Repo $ext_safeprojectname$Repo,
            ILogger<Update$ext_safeprojectname$CommandHandler> logger)
        {
            _mediator = mediator;
            _$ext_safeprojectname$Repo = $ext_safeprojectname$Repo;
            _logger = logger;
        }
        public async Task<Application.DTO.$ext_safeprojectname$> Handle(Update$ext_safeprojectname$Command request, CancellationToken cancellationToken)
        {
            
            //Get current $ext_safeprojectname$ from DB (we can also load the "old" values from the client. Here, I've chosen to load from DB). The "current$ext_safeprojectname$" is used to detect changes and run appropriate business rules
            var currentDB$ext_safeprojectname$ = await _$ext_safeprojectname$Repo.GetAsync(request.Id);
            if (currentDB$ext_safeprojectname$ == null)
                return null;
            EntityPropertyBag eib = new EntityPropertyBag("main", Guid.NewGuid().ToString()); //TODO: get the "CORRECT" tenant topic name!!
            currentDB$ext_safeprojectname$.SetEntityBag(eib);
            //Aggregate root ($ext_safeprojectname$MetaDataMaster)
            var $ext_safeprojectname$ = new $ext_safeprojectname$.Domain.Aggregates.$ext_safeprojectname$.$ext_safeprojectname$(request.Name, request.Description, request.Quantity, request.Amount, request.IsActive);                       

        //check invariants (Business rules)
        var rulesResult = await currentDB$ext_safeprojectname$.ValidateEntityRules($ext_safeprojectname$);

            _logger.LogInformation($"----- Updating $ext_safeprojectname$: {$ext_safeprojectname$}");

            //check if atleast 1 result in "Dictionary<InvariantType, (EntityUpdateResult, DomainServicesException)> from CheckInvariants is NOT successfull"            
            if ((rulesResult.Values.Where(o => o.Item1 == EntityUpdateResult.BusinessRuleViolation).Count() > 0) || (rulesResult.Values.Where(o => o.Item1 == EntityUpdateResult.NoRecordFoundOrStaleValue).Count() > 0)
                || (rulesResult.Values.Where(o => o.Item1 == EntityUpdateResult.Unchanged).Count() > 0))
            {
                var errors = string.Join(",", rulesResult.Values.Select(o => o.Item2.ToString().ToArray()));
                _logger.LogInformation($"----- Update not done (Reason(s): {errors}) for Source $ext_safeprojectname$: {$ext_safeprojectname$}");
                (($ext_safeprojectname$DataContext)_$ext_safeprojectname$Repo.UnitOfWork).Entry(currentDB$ext_safeprojectname$).State = EntityState.Unchanged; //set everything to unmodified                
                return request;
            }
            else
            {
                await _$ext_safeprojectname$Repo.UnitOfWork
                    .SaveEntitiesAsync(cancellationToken);
                _logger.LogInformation($"----- Updated $ext_safeprojectname$: {$ext_safeprojectname$}");
                return request;
            }            
        }
    }
}
