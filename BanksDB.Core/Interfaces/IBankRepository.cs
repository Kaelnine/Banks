using BanksDB.Core.Dtos;
using BanksDB.Core.Entities;

namespace BanksDB.Core.Interfaces
{
    public interface IBankRepository
    {
        Task<IEnumerable<BankDto>> GetAllAsync();// получение всех банков 
        Task<BankDto> GetByIdAsync(int id);// получение банка по id                
        Task AddAsync(Bank bank);// создание банка 
        Task UpdateAsync(Bank bank);// изменение банка 
        Task DeleteAsync(int bankId);// удаление банка 
    }
}
