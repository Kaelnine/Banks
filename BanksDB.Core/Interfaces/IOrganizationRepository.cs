using BanksDB.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BanksDB.Core.Interfaces
{
    public interface IOrganizationRepository
    {
        Task<IEnumerable<OrganizationDto>> GetAllAsync();// получение всех организаций
        Task<OrganizationDto> GetByIdAsync(int id);// получение организации по id
        Task<IEnumerable<AccountDto>> GetByOrganizationIdAsync(int organizationtId);// получение всех счетов организации        
        Task AddAsync(OrganizationDto organization);// создание организации
        Task UpdateAsync(OrganizationDto organization);// изменение организации
        Task DeleteAsync(int organizationId);// удаление организации       
    }
}
