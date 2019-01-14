using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using M5.API.USER.Framework.Domain;
using MediatR;
using M5.API.USER.Framework.Domain.Aggregates;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace M5.API.USER.Framework.Infrastructure
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
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new PropertyConfiguration());
            modelBuilder.ApplyConfiguration(new LabelConfiguration());

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<Label> Labels { get; set; }
    }

    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("User");
            builder.HasKey(v => v.Id);
            builder.Property(v => v.Phone).HasMaxLength(50).IsRequired(true);
            builder.Property(v => v.Name).HasMaxLength(50).IsRequired(true);
        }
    }

    public class PropertyConfiguration : IEntityTypeConfiguration<Property>
    {
        public void Configure(EntityTypeBuilder<Property> builder)
        {
            builder.ToTable("Property");
            builder.HasKey(v => new { v.UserId, v.Key, v.Value });
            builder.Property(v => v.UserId).IsRequired(true);
            builder.Property(v => v.Key).HasMaxLength(50).IsRequired(true);
            builder.Property(v => v.Value).HasMaxLength(50).IsRequired(true);
            builder.Property(v => v.Title).HasMaxLength(50).IsRequired(true);
        }
    }

    public class LabelConfiguration : IEntityTypeConfiguration<Label>
    {
        public void Configure(EntityTypeBuilder<Label> builder)
        {
            builder.ToTable("Label");
            builder.HasKey(v => v.Id);
            builder.Property(v => v.UserId).IsRequired(true);
            builder.Property(v => v.Title).HasMaxLength(50).IsRequired(true);
        }
    }
}
