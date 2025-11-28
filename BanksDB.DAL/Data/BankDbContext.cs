using BanksDB.Core.Dtos;
using BanksDB.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace BanksDB.DAL.Data
{
    public class BankDbContext : DbContext
    {
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

