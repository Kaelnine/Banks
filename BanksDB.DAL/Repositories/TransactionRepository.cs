using AutoMapper;
using BanksDB.Core.Dtos;
using BanksDB.Core.Entities;
using BanksDB.Core.Interfaces;
using BanksDB.Core.Models.InputModels;
using BanksDB.Core.Models.OutputModels;
using BanksDB.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace DBBanks.DAL.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly IDbContextFactory<BankDbContext> _db;
        private readonly IMapper _mapper;
        public TransactionRepository(IDbContextFactory<BankDbContext> db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        // добавление транзакции
        public async Task AddAsync(Transaction transaction)
        {
            await using var db = await _db.CreateDbContextAsync();
            await db.Transactions.AddAsync(transaction);
            await db.SaveChangesAsync();
        }

        // удаление транзакции
        public async Task DeleteAsync(int transactionId)
        {
            await using var db = await _db.CreateDbContextAsync();
            var transaction = await db.Transactions.FindAsync(transactionId);
            if (transaction == null) return;
            transaction.IsDeleted = true;
            await db.SaveChangesAsync();
        }

        // получение всех транзакций
        public async Task<IEnumerable<TransactionDto>> GetAllAsync()
        {
            await using var db = await _db.CreateDbContextAsync();
            var transactions = await db.Transactions.Where(t => !t.IsDeleted).ToListAsync();
            return _mapper.Map<IEnumerable<TransactionDto>>(transactions);
        }

        // получение всех транзакций счета
        public async Task<IEnumerable<TransactionDto>> GetByAccountIdAsync(int accountId)
        {
            await using var db = await _db.CreateDbContextAsync();
            var transactions = await db.Transactions
                .Include(t => t.Account)
                .Where(t => t.AccountId == accountId && !t.IsDeleted)
                .ToListAsync();
            return _mapper.Map<IEnumerable<TransactionDto>>(transactions);
        }

        // получение всех транзакций счета за день
        public async Task<IEnumerable<TransactionDto>> GetByAccountIdForDayAsync(int accountId, DateTime date)
        {
            await using var db = await _db.CreateDbContextAsync();
            var transactions = await db.Transactions
                .Include(t => t.Account)
                .Where(t => t.AccountId == accountId && t.TransactionDate == date && !t.IsDeleted)
                .ToListAsync();
            return _mapper.Map<IEnumerable<TransactionDto>>(transactions);
        }

        // получение всех транзакций за период
        public async Task<IEnumerable<TransactionDto>> GetByAccountIdForPeriodAsync(int accountId, DateTime startDate, DateTime endDate)
        {
            await using var db = await _db.CreateDbContextAsync();
            var transactions = await db.Transactions
                .Include(t => t.Account)
                .Where(t => t.AccountId == accountId && t.CreatedDate > startDate && t.CreatedDate < endDate && !t.IsDeleted)
                .ToListAsync();
            return _mapper.Map<IEnumerable<TransactionDto>>(transactions);
        }

        // получение транзакции по id
        public async Task<TransactionDto> GetByIdAsync(int id)
        {
            await using var db = await _db.CreateDbContextAsync();
            var transaction = await db.Transactions.FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
            if (transaction == null) return null;
            return new TransactionDto
            {
                Id = transaction.Id,
                AccountId = transaction.AccountId,
                TransactionDate = transaction.TransactionDate,
                Amount = transaction.Amount,
                TransactionType = transaction.TransactionType,
                Description = transaction.Description,
                CounterpartyName = transaction.CounterpartyName,
                CounterpartyAccount = transaction.CounterpartyAccount,
                CounterpartyInn = transaction.CounterpartyInn,
                DocumentNumber = transaction.DocumentNumber,
                //BalanceAfter = transaction.BalanceAfter,
                CreatedDate = transaction.CreatedDate
            };
        }

        public async Task<List<DailySummaryOutputModel>> GetDailySummaryAsync(int accountId, DateTime startDate, DateTime endDate)
        {
            await using var db = await _db.CreateDbContextAsync();
            return await db.Transactions.Where(t => t.AccountId == accountId && t.TransactionDate >= startDate && t.TransactionDate <= endDate && !t.IsDeleted)
                .GroupBy(t => t.TransactionDate.Date)
                .Select(g => new DailySummaryOutputModel
                {
                    Date = g.Key,
                    TotalIncome = g.Where(t => t.TransactionType == "Приход").Sum(t => t.Amount),
                    TotalExpense = g.Where(t => t.TransactionType == "Расход").Sum(t => t.Amount),
                    TransactionCount = g.Count()
                })
                .OrderByDescending(s => s.Date)
                .ToListAsync();
        }

        // изменение транзакции
        public async Task UpdateAsync(Transaction transaction)
        {
            await using var db = await _db.CreateDbContextAsync();
            db.Transactions.Update(transaction);
            await db.SaveChangesAsync();
        }

        public async Task<IEnumerable<TransactionDto>> AddSeveralAsync(IEnumerable<Transaction> transactions)
        {
            await using var db = await _db.CreateDbContextAsync();
            await db.Transactions.AddRangeAsync(transactions);
            await db.SaveChangesAsync();
            return _mapper.Map<IEnumerable<TransactionDto>>(transactions);
        }

        public async Task<bool> IsDuplicateTransactionAsync(TransactionInputModel transaction)
        {
            await using var db = await _db.CreateDbContextAsync();
            return await db.Transactions.AnyAsync(t =>
                t.AccountId == transaction.AccountId &&
                t.DocumentNumber == transaction.DocumentNumber &&
                t.TransactionDate == transaction.TransactionDate &&
                t.Amount == transaction.Amount &&
                !t.IsDeleted);
        }

        public async Task<List<TransactionInputModel>> FilterDuplicateTransactionsAsync(List<TransactionInputModel> transactions)
        {
            if (!transactions.Any()) return transactions;
            await using var db = await _db.CreateDbContextAsync();
            var uniqueTransactions = new List<TransactionInputModel>();
            foreach (var transaction in transactions)
            {
                var isDuplicate = await db.Transactions.AnyAsync(t =>
                    t.AccountId == transaction.AccountId &&
                    t.DocumentNumber == transaction.DocumentNumber &&
                    t.TransactionDate == transaction.TransactionDate &&
                    t.Amount == transaction.Amount &&
                    !t.IsDeleted);
                if (!isDuplicate)
                {
                    uniqueTransactions.Add(transaction);
                }
            }
            return uniqueTransactions;
        }

        public async Task<int> GetDuplicateCountAsync(List<TransactionInputModel> transactions)
        {
            if (!transactions.Any()) return 0;
            await using var db = await _db.CreateDbContextAsync();
            var duplicateCount = 0;
            foreach (var transaction in transactions)
            {
                var isDuplicate = await db.Transactions.AnyAsync(t =>
                    t.AccountId == transaction.AccountId &&
                    t.DocumentNumber == transaction.DocumentNumber &&
                    t.TransactionDate == transaction.TransactionDate &&
                    t.Amount == transaction.Amount &&
                    !t.IsDeleted);
                if (isDuplicate) duplicateCount++;
            }
            return duplicateCount;
        }
    }
}
