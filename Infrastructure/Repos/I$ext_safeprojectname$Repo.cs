using Cloud.$ext_safeprojectname$.Domain.Helpers;
using Cloud.$ext_safeprojectname$.Domain.Interfaces;
using System.Threading.Tasks;

namespace Cloud.$ext_safeprojectname$.Infrastructure.Repos
{
    public interface I$ext_safeprojectname$Repo : IRepository<Domain.Aggregates.$ext_safeprojectname$.$ext_safeprojectname$>
    {
        Domain.Aggregates.$ext_safeprojectname$.$ext_safeprojectname$ Add(Domain.Aggregates.$ext_safeprojectname$.$ext_safeprojectname$ item);
        Task<EntityUpdateResult> Update(Domain.Aggregates.$ext_safeprojectname$.$ext_safeprojectname$ item);
        Task<Domain.Aggregates.$ext_safeprojectname$.$ext_safeprojectname$> GetAsync(int uniqueId);
    }
}
