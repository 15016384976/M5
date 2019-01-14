using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using M5.API.LINK.Framework.Domain;
using MediatR;
using M5.API.LINK.Framework.Domain.Aggregates;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace M5.API.LINK.Framework.Infrastructure
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
            modelBuilder.ApplyConfiguration(new LinkConfiguration());
            modelBuilder.ApplyConfiguration(new ContactConfiguration());

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Link> Links { get; set; }
        public DbSet<Contact> Contacts { get; set; }
    }

    public class LinkConfiguration : IEntityTypeConfiguration<Link>
    {
        public void Configure(EntityTypeBuilder<Link> builder)
        {
            builder.ToTable("Link");
            builder.HasKey(v => v.Id);
            builder.Property(v => v.UserId).IsRequired(true);
        }
    }

    public class ContactConfiguration : IEntityTypeConfiguration<Contact>
    {
        public void Configure(EntityTypeBuilder<Contact> builder)
        {
            builder.ToTable("Contact");
            builder.HasKey(v => v.Id);
            builder.Property(v => v.LinkId).IsRequired(true);
            builder.Property(v => v.UserId).IsRequired(true);
            builder.Property(v => v.Name).HasMaxLength(50).IsRequired(true);
        }
    }
}
