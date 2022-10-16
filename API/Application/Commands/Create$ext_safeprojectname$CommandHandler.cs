using Cloud.$ext_safeprojectname$.API.Application.DTO;
using Cloud.$ext_safeprojectname$.Infrastructure.Repos;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Cloud.$ext_safeprojectname$.API.Application.Commands
{
    public class Create$ext_safeprojectname$CommandHandler : IRequestHandler<Create$ext_safeprojectname$Command, Application.DTO.$ext_safeprojectname$>
    {
        private readonly I$ext_safeprojectname$Repo _$ext_safeprojectname$Repository;
        //private readonly IIdentityService _identityService; //TODO
        private readonly IMediator _mediator;        
        private readonly ILogger<Create$ext_safeprojectname$CommandHandler> _logger;

        public Create$ext_safeprojectname$CommandHandler(IMediator mediator,
            I$ext_safeprojectname$Repo $ext_safeprojectname$Repository,
            ILogger<Create$ext_safeprojectname$CommandHandler> logger)
        {
            _mediator = mediator;
            _$ext_safeprojectname$Repository = $ext_safeprojectname$Repository;
            _logger = logger;
        }

        public async Task<Application.DTO.$ext_safeprojectname$> Handle(Create$ext_safeprojectname$Command request, CancellationToken cancellationToken)
        {
            var $ext_safeprojectname$ = new Domain.Aggregates.$ext_safeprojectname$.$ext_safeprojectname$(request.Name, request.Description, request.Quantity,
                request.Amount, request.IsActive);
            await $ext_safeprojectname$.ValidateEntityRules($ext_safeprojectname$);

            _logger.LogInformation($"----- Creating $ext_safeprojectname$: {$ext_safeprojectname$}");

            _$ext_safeprojectname$Repository.Add($ext_safeprojectname$);

            await _$ext_safeprojectname$Repository.UnitOfWork
                .SaveEntitiesAsync(cancellationToken);
            _logger.LogInformation($"----- Created $ext_safeprojectname$: {$ext_safeprojectname$}");
            //refresh client with the newly created "global $ext_safeprojectname$ id"
            request.Id = $ext_safeprojectname$.Key;
            return request;

        }
    }
}
