using BanksDB.Core.Dtos;
using BanksDB.Core.Entities;

namespace BanksDB.Core.Interfaces
{
    public interface IOrganizationRepository
    {
        Task<IEnumerable<OrganizationDto>> GetAllAsync();// получение всех организаций
        Task<OrganizationDto> GetByIdAsync(int id);// получение организации по id 
        Task<IEnumerable<AccountDto>> GetByOrganizationIdAsync(int organizationtId);// получение всех счетов организации        
        Task AddAsync(Organization organization);// создание организации 
        Task UpdateAsync(Organization organization);// изменение организации 
        Task DeleteAsync(int organizationId);// удаление организации       
    }
}
