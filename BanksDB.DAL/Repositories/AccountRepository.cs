using BanksDB.Core.Dtos;
using BanksDB.Core.Interfaces;
using BanksDB.Core.Models.OutputModels;
using BanksDB.Core.Data;
using BanksDB.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata;
using AutoMapper;

namespace DBBanks.DAL.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        //private readonly BankDbContext _db;
        private readonly IDbContextFactory<BankDbContext> _db;
        private readonly IMapper _mapper;

        //public AccountRepository(BankDbContext db) { _db = db; }
        public AccountRepository(IDbContextFactory<BankDbContext> db, IMapper mapper)
        { 
            _db = db; 
            _mapper = mapper;
        }

        // добавление счета
        public async Task AddAsync(Account account)//AccountDto
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

        public async Task<IEnumerable<AccountSummaryDto>> GetAccountSummaryAsync() // AccountSummaryDto
        {
            await using var db = await _db.CreateDbContextAsync();
            return await db.Accounts
                .Where(a => !a.IsDeleted)
                .Include(a => a.Organization)
                .Include(a => a.Bank)                
                .Select(a => new AccountSummaryDto
                {
                    Id = a.Id,
                    OrganizationName = a.Organization.Name,
                    OrganizationInn = a.Organization.Inn,
                    BankName = a.Bank.Name,
                    BankBik = a.Bank.Bik,
                    AccountNumber = a.AccountNumber,
                    CurrentBalance = a.CurrentBalance                    
                })
                .ToListAsync();
            //return await db.AccountSummaries.Select(a => new AccountSummaryDto
            ////return await _db.AccountSummaries.Select(a => new AccountSummaryDto
            //{
            //    OrganizationName = a.OrganizationName,
            //    OrganizationInn = a.OrganizationInn,
            //    BankName = a.BankName,
            //    BankBik = a.BankBik,
            //    AccountNumber = a.AccountNumber,
            //    CurrentBalance = a.CurrentBalance,
            //    //AccountType = a.AccountType
            //}).ToListAsync();
        }

        // получение всех счетов
        public async Task<IEnumerable<AccountDto>> GetAllAsync() // AccountDto
        {
            await using var db = await _db.CreateDbContextAsync();
            //return await _db.Accounts.Where(a => !a.IsDeleted).ToListAsync();
            //return await db.Accounts.Where(a => !a.IsDeleted).ToListAsync();
            var accounts = await db.Accounts
                .Include(a => a.Bank)
                .Include(a => a.Organization)
                .Where(a => !a.IsDeleted)
                .ToListAsync();
            return _mapper.Map<IEnumerable<AccountDto>>(accounts);
        }       

        // получение счета по id
        public async Task<AccountDto> GetByIdAsync(int id) // AccountDto
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


            //return await _db.Accounts.FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
            //return await db.Accounts.FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);

        }               

        // изменение счета
        public async Task UpdateAsync(Account account) // AccountDto
        {
            await using var db = await _db.CreateDbContextAsync();
            //_db.Accounts.Update(account);
            //await _db.SaveChangesAsync();
            db.Accounts.Update(account);
            await db.SaveChangesAsync();
        }

        // получение всех счетов организации
        public async Task<IEnumerable<AccountDto>> GetByOrganizationIdAsync(int organizationtId) // AccountDto
        {
            await using var db = await _db.CreateDbContextAsync();
            //return await _db.Accounts.Where(a => a.OrganizationId == organizationtId).ToListAsync();
            //return await db.Accounts.Where(a => a.OrganizationId == organizationtId).ToListAsync();
            var accounts = await db.Accounts
                .Include(a => a.Bank)
                .Include(a => a.Organization)
                .Where(a => !a.IsDeleted)
                .ToListAsync();
            return _mapper.Map<IEnumerable<AccountDto>>(accounts);
        }

    }
}
