using BanksDB.Core.Dtos;
using BanksDB.Core.Entities;
using BanksDB.Core.Models.InputModels;
using BanksDB.Core.Models.OutputModels;
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
        Task AddAsync(Transaction transaction);// создание транзакции // TransactionDto
        Task UpdateAsync(Transaction transaction);// изменение транзакции TransactionDto
        Task DeleteAsync(int transactionId);// удаление транзакции
        //Task AddSeveralAsync(IEnumerable<Transaction> transactions);
        Task<IEnumerable<TransactionDto>> AddSeveralAsync(IEnumerable<Transaction> transactions);// создание списка транзакций
        Task<List<DailySummaryOutputModel>> GetDailySummaryAsync(int accountId, DateTime startDate, DateTime endDate);// получение общих прихода и расхода по дням
        Task<bool> IsDuplicateTransactionAsync(TransactionInputModel transaction);
        Task<List<TransactionInputModel>> FilterDuplicateTransactionsAsync(List<TransactionInputModel> transactions);
        Task<int> GetDuplicateCountAsync(List<TransactionInputModel> transactions);
    }
}
