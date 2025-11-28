using AutoMapper;
using BanksDB.BLL.Interfaces;
using BanksDB.Core.Entities;
using BanksDB.Core.Interfaces;
using BanksDB.Core.Models.OutputModels;
using BanksDB.DAL.Data;
using Microsoft.EntityFrameworkCore;

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
        public async Task AddAsync(Account account)
        {
            await _accountRepository.AddAsync(account);
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

        // получение всех счетов короткая информация
        public async Task<List<AccountOutputModel>> GetAllAsync()
        {
            var accounts = await _accountRepository.GetAllAsync();
            return _mapper.Map<List<AccountOutputModel>>(accounts);
        }

        // получение всех счетов полная информация
        public async Task<List<Account>> GetAllFullInfoAsync()
        {
            var accounts = await _accountRepository.GetAllAsync();
            return accounts.ToList();
        }

        // получение счета по id
        public async Task<AccountOutputModel> GetByIdAsync(int id)
        {
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
            return _mapper.Map<AccountOutputModel>(account);
        }

        // получение всех счетов организации
        public async Task<List<AccountOutputModel>> GetByOrganizationIdAsync(int organizationId)
        {
            var acounts = await _accountRepository.GetByOrganizationIdAsync(organizationId);
            return _mapper.Map<List<AccountOutputModel>>(acounts);
        }

        // обновление счета        
        public async Task UpdateAsync(Account account)
        {
            await _accountRepository.UpdateAsync(account);
        }
    }
}
