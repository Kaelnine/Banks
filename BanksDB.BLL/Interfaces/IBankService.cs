using BanksDB.Core.Entities;

namespace BanksDB.BLL.Interfaces
{
    public interface IBankService
    {
        Task<List<Bank>> GetAllBanksAsync();
        Task<Bank> GetBankByIdAsync(int id);
        Task AddBankAsync(Bank bank);
        Task UpdateBankAsync(Bank bank);
        Task DeleteBankAsync(int id);
    }
}
