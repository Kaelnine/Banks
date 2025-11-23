using BanksDB.Core.Entities;
using BanksDB.Core.Models.InputModels;
using BanksDB.Core.Models.OutputModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BanksDB.BLL.Interfaces
{
    public interface IAccountService
    {
        Task<List<AccountOutputModel>> GetAllAsync();
        Task<List<Account>> GetAllFullInfoAsync();
        Task<AccountOutputModel> GetByIdAsync(int id);
        Task<List<AccountOutputModel>> GetAccountSummaryAsync();
        //Task AddAsync(AccountInputModel inputModel);
        Task AddAsync(Account account);
        //Task UpdateAsync(int id, AccountInputModel inputModel);
        Task UpdateAsync(Account account);
        Task DeleteAsync(int id);
        Task<List<AccountOutputModel>> GetByOrganizationIdAsync(int organizationId);
    }
}
