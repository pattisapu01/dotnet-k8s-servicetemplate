using Autofac;
using Cloud.$ext_safeprojectname$.Infrastructure.Repos;

namespace Cloud.$ext_safeprojectname$.API.Infrastructure.AutofacModules
{
    public class ApplicationModule
        : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            
            builder.RegisterType<$ext_safeprojectname$Repo>()
                .As<I$ext_safeprojectname$Repo>()
                .InstancePerLifetimeScope();

        }
    }
}
