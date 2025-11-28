using BanksDB.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BanksDB.Core.Configurations
{
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.ToTable("Accounts");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.HasOne(a => a.Organization)
                .WithMany(o => o.Accounts)
                .HasForeignKey(a => a.OrganizationId);

            builder.HasOne(a => a.Bank)
                .WithMany(b => b.Accounts)
                .HasForeignKey(a => a.BankId);

            builder.Property(a => a.IsDeleted).HasDefaultValue(false);
        }
    }
}
