using BanksDB.Core.Dtos;
using BanksDB.Core.Entities;
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
        Task<IEnumerable<Account>> GetAllAsync();// получение всех счетов // AccountDto
        Task<AccountDto> GetByIdAsync(int id);// получение счета по id // AccountDto              
        Task AddAsync(Account account);// создание счета //AccountDto
        Task UpdateAsync(Account account);// изменение счета // AccountDto
        Task DeleteAsync(int bankId);// удаление счета
        Task<IEnumerable<AccountSummaryDto>> GetAccountSummaryAsync();// получение сводки по счетам // AccountSummaryDto
        Task<IEnumerable<AccountDto>> GetByOrganizationIdAsync(int organizationtId);// получение всех счетов организации // AccountDto
    }
}
