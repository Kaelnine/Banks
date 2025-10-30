using BanksDB.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BanksDB.Core.Interfaces
{
    public interface IAccountRepository
    {
        Task<IEnumerable<AccountDto>> GetAllAsync();// получение всех счетов
        Task<AccountDto> GetByIdAsync(int id);// получение счета по id                
        Task AddAsync(AccountDto account);// создание счета
        Task UpdateAsync(AccountDto account);// изменение счета
        Task DeleteAsync(int bankId);// удаление счета
    }
}
