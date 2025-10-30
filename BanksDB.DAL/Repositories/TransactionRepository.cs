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
        public Task<TransactionDto> AddAsync(TransactionDto transaction)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TransactionDto>> AddSeveralAsync(IEnumerable<TransactionDto> transactions)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TransactionDto>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TransactionDto>> GetByAccountsIdAsync(int accountId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TransactionDto>> GetByAccountsIdForDayAsync(int accountId, DateTime date)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TransactionDto>> GetByAccountsIdForPeriodAsync(int accountId, DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TransactionDto>> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<TransactionDto> UpdateAsync(TransactionDto transaction)
        {
            throw new NotImplementedException();
        }
    }
}
