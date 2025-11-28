using AutoMapper;
using BanksDB.Core.Dtos;
using BanksDB.Core.Entities;
using BanksDB.Core.Interfaces;
using BanksDB.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace DBBanks.DAL.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IDbContextFactory<BankDbContext> _db;
        private readonly IMapper _mapper;

        public AccountRepository(IDbContextFactory<BankDbContext> db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        // добавление счета
        public async Task AddAsync(Account account)
        {
            await using var db = await _db.CreateDbContextAsync();
            await db.Accounts.AddAsync(account);
            await db.SaveChangesAsync();
        }

        // удаление счета
        public async Task DeleteAsync(int id)
        {
            await using var db = await _db.CreateDbContextAsync();
            var account = await db.Accounts.FindAsync(id);
            if (account == null) return;
            account.IsDeleted = true;
            await db.SaveChangesAsync();
        }

        public async Task<IEnumerable<AccountSummaryDto>> GetAccountSummaryAsync()
        {
            await using var db = await _db.CreateDbContextAsync();
            return await db.Accounts
                .Where(a => !a.IsDeleted)
                .Include(a => a.Organization)
                .Include(a => a.Bank)
                .Select(a => new AccountSummaryDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    OrganizationName = a.Organization.Name,
                    OrganizationInn = a.Organization.Inn,
                    BankName = a.Bank.Name,
                    BankBik = a.Bank.Bik,
                    AccountNumber = a.AccountNumber,
                    CurrentBalance = a.CurrentBalance
                })
                .ToListAsync();
        }

        // получение всех счетов        
        public async Task<IEnumerable<Account>> GetAllAsync()
        {
            await using var db = await _db.CreateDbContextAsync();
            var accounts = await db.Accounts
                .Include(a => a.Bank)
                .Include(a => a.Organization)
                .Where(a => !a.IsDeleted)
                .ToListAsync();
            return accounts;
        }

        // получение счета по id
        public async Task<AccountDto> GetByIdAsync(int id)
        {
            using var db = await _db.CreateDbContextAsync();
            var account = await db.Accounts
                .Include(a => a.Organization)
                .Include(a => a.Bank)
                .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
            if (account == null) return null;
            return new AccountDto
            {
                Id = account.Id,
                Name = account.Name,
                OrganizationId = account.OrganizationId,
                BankId = account.BankId,
                BankName = account.Bank.Name,
                AccountNumber = account.AccountNumber,
                CurrentBalance = account.CurrentBalance,
                UpdateAccount = account.UpdateAccount
            };
        }

        // изменение счета
        public async Task UpdateAsync(Account account)
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
            var accounts = await db.Accounts
                .Include(a => a.Bank)
                .Include(a => a.Organization)
                .Where(a => !a.IsDeleted)
                .ToListAsync();
            return _mapper.Map<IEnumerable<AccountDto>>(accounts);
        }

    }
}
