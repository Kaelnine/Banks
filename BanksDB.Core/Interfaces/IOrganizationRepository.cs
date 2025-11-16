using BanksDB.Core.Dtos;
using BanksDB.Core.Entities;
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
        Task<OrganizationDto> GetByIdAsync(int id);// получение организации по id // OrganizationDto
        Task<IEnumerable<AccountDto>> GetByOrganizationIdAsync(int organizationtId);// получение всех счетов организации        
        Task AddAsync(Organization organization);// создание организации // OrganizationDto
        Task UpdateAsync(Organization organization);// изменение организации // OrganizationDto
        Task DeleteAsync(int organizationId);// удаление организации       
    }
}
