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

        public async Task<decimal> CalculateBalanceAsync(int accountId, DateTime? asOfDate = null)
        {
            await using var db = await _db.CreateDbContextAsync();
            var account = await db.Accounts.FirstOrDefaultAsync(a => a.Id == accountId);
            if (account == null) return 0;
            var balance = account.InitialBalance;
            var query = db.Transactions.Where(t =>
            t.AccountId == accountId &&
            !t.IsDeleted);
            if (asOfDate.HasValue)
            {
                query = query.Where(t => t.TransactionDate.Date == asOfDate.Value.Date);
            }
            var transactions = await query.ToListAsync();
            foreach (var transaction in transactions)
            {
                if (transaction.TransactionType == "Приход")
                    balance += transaction.Amount;
                else if (transaction.TransactionType == "Расход")
                    balance -= transaction.Amount;
            }
            return balance;
        }

        public async Task<decimal> GetBalanceAsOfDateAsync(int accountId, DateTime date)
        {
            return await CalculateBalanceAsync(accountId, date);
        }

        public async Task<List<BalanceHistoryItem>> GetBalanceHistoryAsync(int accountId, DateTime startDate, DateTime endDate)
        {
            await using var db = await _db.CreateDbContextAsync();
            var account = await db.Accounts.FirstOrDefaultAsync(a => a.Id == accountId);
            if (account == null) return new List<BalanceHistoryItem>();
            var history = new List<BalanceHistoryItem>();
            var currentDate = startDate.Date;
            var initialBalance = await CalculateBalanceAsync(accountId, startDate.AddDays(-1));
            while (currentDate <= endDate)
            {
                var dayTransactions = await db.Transactions
                    .Where(t => t.AccountId == accountId &&
                        t.TransactionDate == currentDate &&
                        !t.IsDeleted)
                    .ToListAsync();
                var dayIncome = dayTransactions
                    .Where(t => t.TransactionType == "Приход")
                    .Sum(t => t.Amount);
                var dayExpense = dayTransactions
                    .Where(t => t.TransactionType == "Расход")
                    .Sum(t => t.Amount);
                var dayBalance = initialBalance + dayIncome - dayExpense;
                history.Add(new BalanceHistoryItem
                {
                    Date = currentDate,
                    Balance = dayBalance,
                    Income = dayIncome,
                    Expense = dayExpense
                });
                initialBalance = dayBalance;
                currentDate= currentDate.AddDays(1);
            }
            return history;
        }

        public async Task<decimal>GetBalanceAtDateAsync(int accountId, DateTime date)
        {
            return await CalculateBalanceAsync(accountId, date);
        }
    }
}
