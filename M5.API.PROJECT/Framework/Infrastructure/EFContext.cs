using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using M5.API.PROJECT.Framework.Domain;
using MediatR;
using M5.API.PROJECT.Framework.Domain.Aggregates;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace M5.API.PROJECT.Framework.Infrastructure
{
    public class EFContext : DbContext, IWorkUnit
    {
        private readonly IMediator _mediator;

        public EFContext(IMediator mediator, DbContextOptions<EFContext> options) : base(options)
        {
            _mediator = mediator;
        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            await _mediator.DispatchDomainEventsAsync(this);
            await base.SaveChangesAsync();
            return true;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ProjectConfiguration());
            modelBuilder.ApplyConfiguration(new PropertyConfiguration());
            modelBuilder.ApplyConfiguration(new MemberConfiguration());

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Project> Projects { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<Member> Members { get; set; }
    }

    public class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.ToTable("Project");
            builder.HasKey(v => v.Id);
            builder.Property(v => v.Name).HasMaxLength(50).IsRequired(true);
            builder.Property(v => v.UserId).IsRequired(true);
        }
    }

    public class PropertyConfiguration : IEntityTypeConfiguration<Property>
    {
        public void Configure(EntityTypeBuilder<Property> builder)
        {
            builder.ToTable("Property");
            builder.HasKey(v => new { v.ProjectId, v.Key, v.Value });
            builder.Property(v => v.ProjectId).IsRequired(true);
            builder.Property(v => v.Key).HasMaxLength(50).IsRequired(true);
            builder.Property(v => v.Value).HasMaxLength(50).IsRequired(true);
            builder.Property(v => v.Title).HasMaxLength(50).IsRequired(true);
        }
    }

    public class MemberConfiguration : IEntityTypeConfiguration<Member>
    {
        public void Configure(EntityTypeBuilder<Member> builder)
        {
            builder.ToTable("Member");
            builder.HasKey(v => v.Id);
            builder.Property(v => v.ProjectId).IsRequired(true);
            builder.Property(v => v.UserId).IsRequired(true);
            builder.Property(v => v.Name).HasMaxLength(50).IsRequired(true);
        }
    }
}
