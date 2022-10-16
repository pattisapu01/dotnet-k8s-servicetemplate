using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cloud.$ext_safeprojectname$.Infrastructure.EntityConfigurations
{
    public class $ext_safeprojectname$Configuration
        : IEntityTypeConfiguration<Domain.Aggregates.$ext_safeprojectname$.$ext_safeprojectname$>
    {
        public void Configure(EntityTypeBuilder<Domain.Aggregates.$ext_safeprojectname$.$ext_safeprojectname$> builder)
        {
            builder.ToTable("$ext_safeprojectname$".ToLower()).Property(p => p.Key)
                .HasColumnName("id")
                .ValueGeneratedOnAdd(); //configure as "Identity" column
            builder.Ignore(a => a.DomainEvents);
            builder.Ignore(i => i.IsConsistent);
            builder.Ignore(i => i.ModifiedDate);            
            builder.HasKey(a => a.Key);                                  
            builder.Property(o => o.Name).IsRequired();            
        }
    }
}
