using BanksDB.Core.Dtos;
using BanksDB.Core.Interfaces;
using BanksDB.Core.Models.OutputModels;
using BanksDB.DAL.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBBanks.DAL.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        //private readonly BankDbContext _db;
        private readonly IDbContextFactory<BankDbContext> _db;

        //public AccountRepository(BankDbContext db) { _db = db; }
        public AccountRepository(IDbContextFactory<BankDbContext> db) { _db = db; }

        // добавление счета
        public async Task AddAsync(AccountDto account)
        {
            await using var db = await _db.CreateDbContextAsync();
            //await _db.Accounts.AddAsync(account);
            //await _db.SaveChangesAsync();
            await db.Accounts.AddAsync(account);
            await db.SaveChangesAsync();
        }

        // удаление счета
        public async Task DeleteAsync(int id)
        {
            await using var db = await _db.CreateDbContextAsync();
            //var account = await _db.Accounts.FindAsync(id);
            //if (account == null) return;
            //account.IsDeleted = true;
            //await _db.SaveChangesAsync();
            var account = await db.Accounts.FindAsync(id);
            if (account == null) return;
            account.IsDeleted = true;
            await db.SaveChangesAsync();
        }

        public async Task<IEnumerable<AccountSummaryDto>> GetAccountSummaryAsync()
        {
            await using var db = await _db.CreateDbContextAsync();
            return await db.AccountSummaries.Select(a => new AccountSummaryDto
            //return await _db.AccountSummaries.Select(a => new AccountSummaryDto
            {
                OrganizationName = a.OrganizationName,
                OrganizationInn = a.OrganizationInn,
                BankName = a.BankName,
                BankBik = a.BankBik,
                AccountNumber = a.AccountNumber,
                CurrentBalance = a.CurrentBalance,
                //AccountType = a.AccountType
            }).ToListAsync();
        }

        // получение всех счетов
        public async Task<IEnumerable<AccountDto>> GetAllAsync()
        {
            await using var db = await _db.CreateDbContextAsync();
            //return await _db.Accounts.Where(a => !a.IsDeleted).ToListAsync();
            return await db.Accounts.Where(a => !a.IsDeleted).ToListAsync();
        }       

        // получение счета по id
        public async Task<AccountDto> GetByIdAsync(int id)
        {
            await using var db = await _db.CreateDbContextAsync();
            //return await _db.Accounts.FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
            return await db.Accounts.FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
        }               

        // изменение счета
        public async Task UpdateAsync(AccountDto account)
        {
            await using var db = await _db.CreateDbContextAsync();
            //_db.Accounts.Update(account);
            //await _db.SaveChangesAsync();
            db.Accounts.Update(account);
            await db.SaveChangesAsync();
        }

        // получение всех счетов организации
        public async Task<IEnumerable<AccountDto>> GetByOrganizationIdAsync(int organizationtId)
        {
            await using var db = await _db.CreateDbContextAsync();
            //return await _db.Accounts.Where(a => a.OrganizationId == organizationtId).ToListAsync();
            return await db.Accounts.Where(a => a.OrganizationId == organizationtId).ToListAsync();
        }

    }
}
