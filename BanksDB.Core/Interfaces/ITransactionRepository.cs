using BanksDB.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BanksDB.Core.Interfaces
{
    public interface ITransactionRepository
    {
        Task<IEnumerable<TransactionDto>> GetAllAsync();// получение всех транзакций
        Task<TransactionDto> GetByIdAsync(int id);// получение транзакции по id
        Task<IEnumerable<TransactionDto>> GetByAccountIdAsync(int accountId);// получение всех транзакций счета
        Task<IEnumerable<TransactionDto>> GetByAccountIdForDayAsync(int accountId, DateTime date);// получение транзакций за день
        Task<IEnumerable<TransactionDto>> GetByAccountIdForPeriodAsync(int accountId, DateTime startDate, DateTime endDate);// получение транзакций за период
        Task AddAsync(TransactionDto transaction);// создание транзакции
        Task UpdateAsync(TransactionDto transaction);// изменение транзакции
        Task DeleteAsync(int transactionId);// удаление транзакции
        Task AddSeveralAsync(IEnumerable<TransactionDto> transactions);// создание списка транзакций
    }
}
