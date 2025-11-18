using BanksDB.Core.Dtos;
using BanksDB.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BanksDB.Core.Data
{
    public class BankDbContext : DbContext
    {
        //public BankDbContext(DbContextOptions<BankDbContext> options) : base(options) { }
        //public DbSet<AccountDto> Accounts { get; set; }
        //public DbSet<OrganizationDto> Organizations { get; set; }
        //public DbSet<BankDto> Banks { get; set; }
        //public DbSet<TransactionDto> Transactions { get; set; }
        //public DbSet<AccountSummaryDto> AccountSummaries { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);
        //    modelBuilder.Entity<AccountDto>().ToTable("Accounts");
        //    modelBuilder.Entity<OrganizationDto>().ToTable("Organizations");
        //    modelBuilder.Entity<BankDto>().ToTable("Banks");
        //    modelBuilder.Entity<TransactionDto>().ToTable("Transactions");
        //    modelBuilder.Entity<AccountSummaryDto>().HasNoKey().ToView("AccountSummary");
        //}
        public BankDbContext(DbContextOptions<BankDbContext> options) : base(options) { }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Bank> Banks { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<User> Users { get; set; }

        public DbSet<AccountSummaryDto> AccountSummaries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BankDbContext).Assembly);

            modelBuilder.Entity<AccountSummary>().HasNoKey().ToView("AccountSummary");
        }
    }
}

