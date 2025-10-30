using BanksDB.Core.Dtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BanksDB.DAL.Data
{
    public class BankDbContext : DbContext
    {
        public BankDbContext(DbContextOptions<BankDbContext> options) : base(options) { }
        public DbSet<AccountDto> Accounts { get; set; }
        public DbSet<OrganizationDto> Organizations { get; set; }
        public DbSet<BankDto> Banks { get; set; }
        public DbSet<TransactionDto> Transactions { get; set; }
        public DbSet<AccountSummaryDto> AccountSummaries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<AccountDto>().ToTable("Accounts");
            modelBuilder.Entity<OrganizationDto>().ToTable("Organizations");
            modelBuilder.Entity<BankDto>().ToTable("Banks");
            modelBuilder.Entity<TransactionDto>().ToTable("Transactions");
            modelBuilder.Entity<AccountSummaryDto>().ToView("AccountSummary");
        }
    }
}

