using Autofac;
using FluentValidation;
using MediatR;
using Cloud.$ext_safeprojectname$.API.Application.Behaviors;
using Cloud.$ext_safeprojectname$.API.Application.Commands;
using Cloud.$ext_safeprojectname$.API.Application.Validations;
using Cloud.$ext_safeprojectname$.Infrastructure.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Cloud.$ext_safeprojectname$.API.Infrastructure.AutofacModules
{
    public class MediatorModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly)
                .AsImplementedInterfaces();

            #region mediator commands and handlers
            // This registers ALL 'commands' in the assembly
            builder.RegisterAssemblyTypes(typeof(Create$ext_safeprojectname$Command).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(IRequestHandler<,>));
            

            //This registers all validators in the assembly
            builder.RegisterAssemblyTypes(typeof(Create$ext_safeprojectname$CommandValidator).GetTypeInfo().Assembly)
                .Where(t => t.IsClosedTypeOf(typeof(IValidator<>)))
                .AsImplementedInterfaces();
            
            builder.Register<ServiceFactory>(context =>
            {
                var componentContext = context.Resolve<IComponentContext>();
                return t => { object o; return componentContext.TryResolve(t, out o) ? o : null; };
            });

            //the following behaviors execute before the actual "command handler" is executed. ex: LoggingBehavior gives us the ability to log 
            //all calls to commands and their handlers. "ValidationBehavior" applies validation rules for specific updates to "Entities" before
            /// the actual "handler" is called. This gives us a clean way of implementing "validations" on aggregates.
            builder.RegisterGeneric(typeof(LoggingBehavior<,>)).As(typeof(IPipelineBehavior<,>));
            builder.RegisterGeneric(typeof(ValidationBehavior<,>)).As(typeof(IPipelineBehavior<,>));
            builder.RegisterGeneric(typeof($ext_safeprojectname$TransactionBehavior<,>)).As(typeof(IPipelineBehavior<,>));

            #endregion
        }
    }
}
