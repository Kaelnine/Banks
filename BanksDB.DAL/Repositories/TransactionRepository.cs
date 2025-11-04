using BanksDB.Core.Dtos;
using BanksDB.Core.Interfaces;
using BanksDB.DAL.Data;
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
        private readonly BankDbContext _db;
        public TransactionRepository(BankDbContext db) { _db = db; }

        // добавление транзакции
        public async Task AddAsync(TransactionDto transaction)
        {
            await _db.Transactions.AddAsync(transaction);
            await _db.SaveChangesAsync();
        }

        // добавление списка транзакций
        public async Task AddSeveralAsync(IEnumerable<TransactionDto> transactions)
        {
            await _db.Transactions.AddRangeAsync(transactions);
            await _db.SaveChangesAsync();
        }

        // удаление транзакции
        public async Task DeleteAsync(int transactionId)
        {
            var transaction = await _db.Transactions.FindAsync(transactionId);
            if (transaction == null) return;
            transaction.IsDeleted = true;
            await _db.SaveChangesAsync();
        }

        // получение всех транзакций
        public async Task<IEnumerable<TransactionDto>> GetAllAsync()
        {
            return await _db.Transactions.Where(t => !t.IsDeleted).ToListAsync();
        }

        // получение всех транзакций счета
        public async Task<IEnumerable<TransactionDto>> GetByAccountIdAsync(int accountId)
        {
            return await _db.Transactions.Where(t => t.AccountId == accountId && !t.IsDeleted).ToListAsync();
        }

        // получение всех транзакций счета за день
        public async Task<IEnumerable<TransactionDto>> GetByAccountIdForDayAsync(int accountId, DateTime date)
        {
            return await _db.Transactions.Where(t => t.AccountId == accountId && t.CreatedDate == date && !t.IsDeleted).ToListAsync();
        }

        // получение всех транзакций за период
        public async Task<IEnumerable<TransactionDto>> GetByAccountIdForPeriodAsync(int accountId, DateTime startDate, DateTime endDate)
        {
            return await _db.Transactions.Where(t => t.AccountId == accountId && t.CreatedDate > startDate && t.CreatedDate < endDate && !t.IsDeleted).ToListAsync();
        }

        // получение транзакции по id
        public async Task<TransactionDto> GetByIdAsync(int id)
        {
            return await _db.Transactions.FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
        }

        // изменение транзакции
        public async Task UpdateAsync(TransactionDto transaction)
        {
            _db.Transactions.Update(transaction);
            await _db.SaveChangesAsync();
        }

        Task<IEnumerable<TransactionDto>> ITransactionRepository.AddSeveralAsync(IEnumerable<TransactionDto> transactions)
        {
            throw new NotImplementedException();
        }
    }
}
