using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace Cloud.$ext_safeprojectname$.Infrastructure.Base
{
    public class BaseController<T> : ControllerBase
    {
        protected readonly IMediator _mediator;
        protected readonly ILogger<T> _logger;

        public BaseController(IMediator mediator, ILogger<T> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
    }
}