using Cloud.$ext_safeprojectname$.Domain.Helpers;
using Cloud.$ext_safeprojectname$.Infrastructure.Base;
using Cloud.$ext_safeprojectname$.Infrastructure.DataContexts;
using Cloud.$ext_safeprojectname$.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Cloud.$ext_safeprojectname$.Infrastructure.Repos
{
    public class $ext_safeprojectname$Repo : BaseRepo<$ext_safeprojectname$DataContext, Domain.Aggregates.$ext_safeprojectname$.$ext_safeprojectname$>, I$ext_safeprojectname$Repo
    {
        public $ext_safeprojectname$Repo($ext_safeprojectname$DataContext context) : base(context)
        {

        }
        public override Domain.Aggregates.$ext_safeprojectname$.$ext_safeprojectname$ Add(Domain.Aggregates.$ext_safeprojectname$.$ext_safeprojectname$ item)
        {
            if (!item.IsConsistent)
                throw new Domain.Exceptions.DomainException($"Business rules violation for {item.Name}", null);
            if (item.IsTransient())
            {
                return _context.$ext_safeprojectname$
                    .Add(item)
                    .Entity;
            }

            return item;
        }

        public async Task<Domain.Aggregates.$ext_safeprojectname$.$ext_safeprojectname$> GetAsync(int uniqueId)
        {
            var item = await _context.$ext_safeprojectname$
               .Where(b => b.Key == uniqueId)
               .SingleOrDefaultAsync();

            return item;
        }
      
        public override Task<EntityUpdateResult> Update(Domain.Aggregates.$ext_safeprojectname$.$ext_safeprojectname$ item)
        {
            if (!item.IsConsistent)
                throw new Domain.Exceptions.DomainException($"Business rules violation for {item.Name}", null);
            EFExtensions.DetachLocalEntityBeforeUpdate(item, _context, (innerItem) => innerItem.Key == item.Key);
            _context.$ext_safeprojectname$
                    .Update(item);
            return Task.FromResult(EntityUpdateResult.Successfull);
        }
    }
}
