using Cloud.$ext_safeprojectname$.Domain.Helpers;
using Cloud.$ext_safeprojectname$.Domain.Interfaces;
using Cloud.$ext_safeprojectname$.Infrastructure.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Cloud.$ext_safeprojectname$.Infrastructure.DataContexts
{
    public class $ext_safeprojectname$DataContext : BaseDBContext, IUnitOfWork
    {
        private readonly IConnectionBuilder _connectionStringBuilder;

        public DbSet<Domain.Aggregates.$ext_safeprojectname$.$ext_safeprojectname$> $ext_safeprojectname$ { get; set; }
        public $ext_safeprojectname$DataContext(DbContextOptions<$ext_safeprojectname$DataContext> options, IConnectionBuilder connectionStringBuilder) : base(options) 
        { 
             _connectionStringBuilder = connectionStringBuilder;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            var dbConnString = _connectionStringBuilder.GetDbConnectionString();
            optionsBuilder.UseNpgsql(dbConnString)
                        .UseLowerCaseNamingConvention();

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new $ext_safeprojectname$Configuration());
        }

        public override async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            await base.SaveChangesAsync(cancellationToken);
            this.ChangeTracker.Clear();
            return true;
        }
    }
}
