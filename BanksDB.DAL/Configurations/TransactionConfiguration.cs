using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BanksDB.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BanksDB.Core.Configurations
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            //builder.ToTable("Transactions");
            //builder.HasKey(x => x.Id);

            //builder.HasOne(x => x.Account)
            //    .WithMany(a => a.Transactions)
            //    .HasForeignKey(x => x.AccountId);
            builder.ToTable("Transactions");

            builder.HasKey(t => t.Id);

            builder.HasOne(t => t.Account)
                .WithMany(a => a.Transactions)
                .HasForeignKey(t => t.AccountId);

            builder.Property(t => t.TransactionType).HasMaxLength(100);
            builder.Property(t => t.Description).HasMaxLength(500);
        }
    }
}
