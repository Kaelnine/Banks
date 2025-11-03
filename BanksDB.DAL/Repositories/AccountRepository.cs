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
        private readonly BankDbContext _db;
        public AccountRepository(BankDbContext db) { _db = db; }

        // добавление счета
        public async Task AddAsync(AccountDto account)
        {
            await _db.Accounts.AddAsync(account);
            await _db.SaveChangesAsync();
        }

        // удаление счета
        public async Task DeleteAsync(int id)
        {
            var account = await _db.Accounts.FindAsync(id);
            if (account == null) return;
            account.IsDeleted = true;
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<AccountSummaryDto>> GetAccountSummaryAsync()
        {
            return await _db.AccountSummaries.Select(a => new AccountSummaryDto
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
            return await _db.Accounts.Where(a => !a.IsDeleted).ToListAsync();
        }       

        // получение счета по id
        public async Task<AccountDto> GetByIdAsync(int id)
        {
            return await _db.Accounts.FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
        }               

        // изменение счета
        public async Task UpdateAsync(AccountDto account)
        {
            _db.Accounts.Update(account);
            await _db.SaveChangesAsync();
        }

        // получение всех счетов организации
        public async Task<IEnumerable<AccountDto>> GetByOrganizationIdAsync(int organizationtId)
        {
            return await _db.Accounts.Where(a => a.OrganizationId == organizationtId).ToListAsync();
        }

    }
}
