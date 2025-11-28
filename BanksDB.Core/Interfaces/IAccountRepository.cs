using BanksDB.Core.Dtos;
using BanksDB.Core.Entities;

namespace BanksDB.Core.Interfaces
{
    public interface IAccountRepository
    {
        Task<IEnumerable<Account>> GetAllAsync();// получение всех счетов 
        Task<AccountDto> GetByIdAsync(int id);// получение счета по id            
        Task AddAsync(Account account);// создание счета
        Task UpdateAsync(Account account);// изменение счета 
        Task DeleteAsync(int bankId);// удаление счета
        Task<IEnumerable<AccountSummaryDto>> GetAccountSummaryAsync();// получение сводки по счетам 
        Task<IEnumerable<AccountDto>> GetByOrganizationIdAsync(int organizationtId);// получение всех счетов организации 
    }
}
