using AutoMapper;
using BanksDB.BLL.Interfaces;
using BanksDB.Core.Dtos;
using BanksDB.Core.Interfaces;
using BanksDB.Core.Models.InputModels;
using BanksDB.Core.Models.OutputModels;
using BanksDB.DAL.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BanksDB.BLL.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _mapper;
        private readonly IDbContextFactory<BankDbContext> _db;

        public AccountService(IAccountRepository accountRepository, IMapper mapper, IDbContextFactory<BankDbContext> db)
        {
            _accountRepository = accountRepository;
            _mapper = mapper;
            _db = db;
        }
        // добавление счета
        public async Task AddAsync(AccountInputModel inputModel)
        {
            var accountDto = _mapper.Map<AccountDto>(inputModel);
            await _accountRepository.AddAsync(accountDto);            
        }

        // удаление счета
        public async Task DeleteAsync(int id)
        {
            await _accountRepository.DeleteAsync(id);
        }

        // получение сводки по счетам
        public async Task<List<AccountOutputModel>> GetAccountSummaryAsync()
        {
            var summary = await _accountRepository.GetAccountSummaryAsync();
            return _mapper.Map<List<AccountOutputModel>>(summary);
        }

        // получение всех счетов
        public async Task<List<AccountOutputModel>> GetAllAsync()
        {
            var accounts = await _accountRepository.GetAllAsync();
            return _mapper.Map<List<AccountOutputModel>>(accounts);
        }

        // получение счета по id
        public async Task<AccountOutputModel> GetByIdAsync(int id)
        {
            //await Task.Delay(100);
            await using var db = await _db.CreateDbContextAsync();
            var account = await db.Accounts
            .Where(a => a.Id == id && !a.IsDeleted)
            .Include(a => a.Bank)
            .Include(a => a.Organization)
            //.Include(a => a.AccountType)
            .Select(a => new AccountOutputModel
            {
                Id = a.Id,
                Name = a.Name,
                OrganizationName = a.Organization.Name,
                BankName = a.Bank.Name,
                BankBik = a.Bank.Bik,
                AccountNumber = a.AccountNumber,
                CurrentBalance = a.CurrentBalance,
                //AccountTypeName = a.AccountType.Name
            })
            .FirstOrDefaultAsync();

            //return account;
            //return account;
            //var account = await _accountRepository.GetByIdAsync(id);
            return _mapper.Map<AccountOutputModel>(account);
        }

        // получение всех счетов организации
        public async Task<List<AccountOutputModel>> GetByOrganizationIdAsync(int organizationId)
        {
            var acounts = await _accountRepository.GetByOrganizationIdAsync(organizationId);
            return _mapper.Map<List<AccountOutputModel>>(acounts);
        }

        // обновление счета
        public async Task UpdateAsync(int id, AccountInputModel inputModel)
        {
            var accountDto = _mapper.Map<AccountDto>(inputModel);
            accountDto.Id = id;
            await _accountRepository.UpdateAsync(accountDto);            
        }
    }
}
