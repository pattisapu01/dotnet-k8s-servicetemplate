using Cloud.$ext_safeprojectname$.API.Application.Commands;
using Cloud.$ext_safeprojectname$.Domain.Exceptions;
using Cloud.$ext_safeprojectname$.Infrastructure.Base;
using Cloud.$ext_safeprojectname$.Infrastructure.Extensions;
using Cloud.$ext_safeprojectname$.Infrastructure.Repos;
using Dapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Cloud.$ext_safeprojectname$.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class $ext_safeprojectname$Controller : BaseController<$ext_safeprojectname$Controller>
    {
        private readonly I$ext_safeprojectname$Repo _$ext_safeprojectname$Repo;
        private readonly IConfiguration _config;
        public $ext_safeprojectname$Controller(IMediator mediator, ILogger<$ext_safeprojectname$Controller> logger, 
            I$ext_safeprojectname$Repo $ext_safeprojectname$Repo, IConfiguration configuration) : base(mediator, logger)
        {
            _$ext_safeprojectname$Repo = $ext_safeprojectname$Repo;
            _config = configuration;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces(typeof(ActionResult<Application.DTO.$ext_safeprojectname$>))]
        [SwaggerOperation(
            Summary = "Creates a new $ext_safeprojectname$",
            Description = "Creates a new $ext_safeprojectname$")
        ]
        public async Task<ActionResult<Application.DTO.$ext_safeprojectname$>> Create$ext_safeprojectname$Async([FromBody] Application.DTO.$ext_safeprojectname$ $ext_safeprojectnamelowercase$)
        {
            try
            { 
                var cmd = new Create$ext_safeprojectname$Command() { Amount = $ext_safeprojectnamelowercase$.Amount, Description = $ext_safeprojectnamelowercase$.Description, IsActive = $ext_safeprojectnamelowercase$.IsActive, Name = $ext_safeprojectnamelowercase$.Name, Quantity = $ext_safeprojectnamelowercase$.Quantity };
                using (_logger.BeginScope(new { ControllerCommand = "Create$ext_safeprojectname$Async", Key = cmd.Id }))
                { 
                    return await _mediator.Send(cmd);
                }
            }
            catch (Exception ex) when (ex is FluentValidation.ValidationException or DomainException)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces(typeof(ActionResult<Application.DTO.$ext_safeprojectname$>))]
        [SwaggerOperation(
            Summary = "Updates an $ext_safeprojectname$",
            Description = "Updates an $ext_safeprojectname$")
        ]
        public async Task<ActionResult<Application.DTO.$ext_safeprojectname$>> Update$ext_safeprojectname$Async([FromBody] Application.DTO.$ext_safeprojectname$ $ext_safeprojectnamelowercase$)
        {
            try
            { 
                var cmd = new Update$ext_safeprojectname$Command() { Id = $ext_safeprojectnamelowercase$.Id, Amount = $ext_safeprojectnamelowercase$.Amount, Description = $ext_safeprojectnamelowercase$.Description, IsActive = $ext_safeprojectnamelowercase$.IsActive, Name = $ext_safeprojectnamelowercase$.Name, Quantity = $ext_safeprojectnamelowercase$.Quantity };
                using (_logger.BeginScope(new { ControllerCommand = "Update$ext_safeprojectname$Async", Key = cmd.Id }))
                {
                    var output = await _mediator.Send(cmd);
                    return output == null ? NotFound($"The $ext_safeprojectname$ Id {$ext_safeprojectnamelowercase$.Id} does not exist") : output;
                }  
            }
            catch (Exception ex) when (ex is FluentValidation.ValidationException or DomainException)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Application.DTO.$ext_safeprojectname$>>> GetById(int id)
        {
            var r = await ((DbContext)_$ext_safeprojectname$Repo.UnitOfWork).QueryAsync<Application.DTO.$ext_safeprojectname$>(CancellationToken.None, @"
                select id,name, description, quantity,amount, isactive from $ext_safeprojectname$ where id=@id", new { id = id });
            if (r == null || !r.Any())
                return NotFound($"{id} not found");
            return Ok(r);
        }       
    }
}
