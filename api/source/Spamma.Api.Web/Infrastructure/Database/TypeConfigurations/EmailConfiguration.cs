using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Spamma.Api.Web.Infrastructure.Domain.EmailAggregate.Aggregate;

namespace Spamma.Api.Web.Infrastructure.Database.TypeConfigurations
{
    public class EmailConfiguration : IEntityTypeConfiguration<Email>
    {
        public void Configure(EntityTypeBuilder<Email> builder)
        {
            builder.HasKey(e => e.Id);
            builder.ToTable("Email");
            builder.HasKey(e => e.Id);
            builder.Ignore(e => e.DomainEvents);
            builder.Property(e => e.Id).ValueGeneratedNever();

            builder.OwnsMany(e => e.EmailAddresses, eA =>
            {
                eA.HasKey(e => e.Id);
                eA.ToTable("EmailAddress");
                eA.HasKey(e => e.Id);
                eA.Ignore(e => e.DomainEvents);
                eA.Property(e => e.Id).ValueGeneratedNever();
            });
        }
    }
}