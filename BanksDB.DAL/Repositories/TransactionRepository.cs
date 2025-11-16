using AutoMapper;
using BanksDB.Core.Data;
using BanksDB.Core.Dtos;
using BanksDB.Core.Entities;
using BanksDB.Core.Interfaces;
using BanksDB.Core.Models.OutputModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public async Task AddAsync(Transaction transaction)// TransactionDto
        {
            await using var db = await _db.CreateDbContextAsync();
            //await _db.Transactions.AddAsync(transaction);
            //await _db.SaveChangesAsync();            
            await db.Transactions.AddAsync(transaction);
            await db.SaveChangesAsync();
        }

        // добавление списка транзакций
        public async Task AddSeveralAsync(IEnumerable<Transaction> transactions)
        {
            await using var db = await _db.CreateDbContextAsync();
            //await _db.Transactions.AddRangeAsync(transactions);
            //await _db.SaveChangesAsync();
            await db.Transactions.AddRangeAsync(transactions);
            await db.SaveChangesAsync();
        }

        // удаление транзакции
        public async Task DeleteAsync(int transactionId)
        {
            await using var db = await _db.CreateDbContextAsync();
            //var transaction = await _db.Transactions.FindAsync(transactionId);
            //if (transaction == null) return;
            //transaction.IsDeleted = true;
            //await _db.SaveChangesAsync();
            var transaction = await db.Transactions.FindAsync(transactionId);
            if (transaction == null) return;
            transaction.IsDeleted = true;
            await db.SaveChangesAsync();
        }

        // получение всех транзакций
        public async Task<IEnumerable<TransactionDto>> GetAllAsync()
        {
            await using var db = await _db.CreateDbContextAsync();
            //return await _db.Transactions.Where(t => !t.IsDeleted).ToListAsync();
            //return await db.Transactions.Where(t => !t.IsDeleted).ToListAsync();
            var transactions = await db.Transactions.Where(t => !t.IsDeleted).ToListAsync();
            return _mapper.Map<IEnumerable<TransactionDto>>(transactions);
        }

        // получение всех транзакций счета
        public async Task<IEnumerable<TransactionDto>> GetByAccountIdAsync(int accountId)
        {
            await using var db = await _db.CreateDbContextAsync();
            //return await _db.Transactions.Where(t => t.AccountId == accountId && !t.IsDeleted).ToListAsync();
            return await db.Transactions.Where(t => t.AccountId == accountId && !t.IsDeleted).ToListAsync();
        }

        // получение всех транзакций счета за день
        public async Task<IEnumerable<TransactionDto>> GetByAccountIdForDayAsync(int accountId, DateTime date)
        {
            await using var db = await _db.CreateDbContextAsync();
            //return await _db.Transactions.Where(t => t.AccountId == accountId && t.CreatedDate == date && !t.IsDeleted).ToListAsync();
            return await db.Transactions.Where(t => t.AccountId == accountId && t.CreatedDate == date && !t.IsDeleted).ToListAsync();
        }

        // получение всех транзакций за период
        public async Task<IEnumerable<TransactionDto>> GetByAccountIdForPeriodAsync(int accountId, DateTime startDate, DateTime endDate)
        {
            await using var db = await _db.CreateDbContextAsync();
            //return await _db.Transactions.Where(t => t.AccountId == accountId && t.CreatedDate > startDate && t.CreatedDate < endDate && !t.IsDeleted).ToListAsync();
            return await db.Transactions.Where(t => t.AccountId == accountId && t.CreatedDate > startDate && t.CreatedDate < endDate && !t.IsDeleted).ToListAsync();
        }

        // получение транзакции по id
        public async Task<TransactionDto> GetByIdAsync(int id)
        {
            await using var db = await _db.CreateDbContextAsync();
            //return await _db.Transactions.FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
            return await db.Transactions.FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
        }

        public async Task<List<DailySummaryOutputModel>> GetDailySummaryAsync(int accountId, DateTime startDate, DateTime endDate)
        {
            await using var db = await _db.CreateDbContextAsync();
            //return await _db.Transactions.Where(t => t.AccountId == accountId && t.TransactionDate >= startDate &&  t.TransactionDate <= endDate && !t.IsDeleted)
            //    .GroupBy(t => t.TransactionDate.Date)
            //    .Select(g => new DailySummaryOutputModel
            //    {
            //        Date = g.Key,
            //        TotalIncome = g.Where(t => t.TransactionType == "Приход").Sum(t => t.Amount),
            //        TotalExpense = g.Where(t => t.TransactionType == "Расход").Sum(t => t.Amount),
            //        TransactionCount = g.Count()
            //    })
            //    .OrderByDescending(s => s.Date)
            //    .ToListAsync();
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
        public async Task UpdateAsync(TransactionDto transaction)
        {
            await using var db = await _db.CreateDbContextAsync();
            //_db.Transactions.Update(transaction);
            //await _db.SaveChangesAsync();
            db.Transactions.Update(transaction);
            await db.SaveChangesAsync();
        }

        Task<IEnumerable<TransactionDto>> ITransactionRepository.AddSeveralAsync(IEnumerable<TransactionDto> transactions)
        {
            throw new NotImplementedException();
        }
    }
}
