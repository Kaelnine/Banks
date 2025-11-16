using BanksDB.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BanksDB.Core.Configurations
{
    public class BankConfiguration : IEntityTypeConfiguration<Bank>
    {
        public void Configure(EntityTypeBuilder<Bank> builder)
        {
            //builder.ToTable("Banks");
            //builder.HasKey(x => x.Id);
            builder.ToTable("Banks");

            builder.HasKey(b => b.Id);

            builder.Property(b => b.Name).IsRequired().HasMaxLength(200);

            builder.Property(b => b.IsDeleted).HasDefaultValue(false);
        }
    }
}
