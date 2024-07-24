using Banking.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Banking.Infrastructure.Core
{
    internal class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.ToTable(nameof(Account));

            builder.HasKey(e => e.Id);

            builder.HasIndex(e => e.PhoneNumber).IsUnique();

            builder.Property(e => e.Id).HasColumnName("Id").ValueGeneratedNever().IsRequired();
            builder.Property(e => e.Name).HasColumnName("Name").IsRequired();
            builder.Property(e => e.PhoneNumber).HasColumnName("PhoneNumber").IsRequired();
        }
    }
}
