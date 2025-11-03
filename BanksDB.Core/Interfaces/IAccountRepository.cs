using BanksDB.Core.Dtos;
using BanksDB.Core.Models.OutputModels;
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
        Task<IEnumerable<AccountSummaryDto>> GetAccountSummaryAsync();// получение сводки по счетам
        Task<IEnumerable<AccountDto>> GetByOrganizationIdAsync(int organizationtId);// получение всех счетов организации
    }
}
