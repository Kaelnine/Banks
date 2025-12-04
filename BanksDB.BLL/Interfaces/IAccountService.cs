using BanksDB.Core.Entities;
using BanksDB.Core.Models.OutputModels;

namespace BanksDB.BLL.Interfaces
{
    public interface IAccountService
    {
        Task<List<AccountOutputModel>> GetAllAsync();
        Task<List<Account>> GetAllFullInfoAsync();
        Task<AccountOutputModel> GetByIdAsync(int id);
        Task<List<AccountOutputModel>> GetAccountSummaryAsync();
        Task AddAsync(Account account);
        Task UpdateAsync(Account account);
        Task DeleteAsync(int id);
        Task<List<AccountOutputModel>> GetByOrganizationIdAsync(int organizationId);
        Task<decimal> CalculateBalanceAsync(int accountId, DateTime? asOfDate = null);
    }
    public class BalanceHistoryItem
    {
        public DateTime Date { get; set; }
        public decimal Balance { get; set; }
        public decimal Income { get; set; }
        public decimal Expense { get; set; }
    }
}
