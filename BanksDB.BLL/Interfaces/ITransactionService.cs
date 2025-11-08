using BanksDB.Core.Models.InputModels;
using BanksDB.Core.Models.OutputModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BanksDB.BLL.Interfaces
{
    public interface ITransactionService
    {
        Task<List<TransactionOutputModel>> GetAllTransactionsAsync();
        Task<TransactionOutputModel> GetTransactionByIdAsync(int id);
        Task<List<TransactionOutputModel>> GetTransactionsByAccountAsync(int accountId);
        Task<List<TransactionOutputModel>> GetTransactionsByAccountAndDateAsync(int accountId, DateTime date);
        Task<List<TransactionOutputModel>> GetTransactionsByAccountAndPeriodAsync(int accountId, DateTime startDate, DateTime endDate);
        Task<List<DailySummaryOutputModel>> GetDailySummaryAsync(int accountId, DateTime startDate, DateTime endDate);
        Task AddTransactionAsync(TransactionInputModel inputModel);
        Task<TransactionOutputModel> UpdateTransactionAsync(int id, TransactionInputModel inputModel);
        Task DeleteTransactionAsync(int id);
        Task<List<TransactionOutputModel>> AddSeveralTransactionsAsync(List<TransactionInputModel> inputModels);        
    }
}
