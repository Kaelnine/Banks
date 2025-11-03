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
        Task<AccountOutputModel> GetByIdAsync(int id);
        Task<List<AccountOutputModel>> GetAccountSummaryAsync();
        Task AddAsync(AccountInputModel inputModel);
        Task UpdateAsync(int id, AccountInputModel inputModel);
        Task DeleteAsync(int id);
        Task<List<AccountOutputModel>> GetByOrganizationIdAsync(int organizationId);
    }
}
