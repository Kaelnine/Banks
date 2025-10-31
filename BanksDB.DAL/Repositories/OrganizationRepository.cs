using BanksDB.Core.Dtos;
using BanksDB.Core.Interfaces;
using BanksDB.DAL.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBBanks.DAL.Repositories
{
    public class OrganizationRepository : IOrganizationRepository
    {
        private readonly BankDbContext _db;
        public OrganizationRepository(BankDbContext db) { _db = db; }

        // добавление организации
        public async Task AddAsync(OrganizationDto organization)
        {
            await _db.Organizations.AddAsync(organization);
            await _db.SaveChangesAsync();
        }

        // удаление организации
        public async Task DeleteAsync(int id)
        {
            var organization = await _db.Organizations.FindAsync(id);
            if (organization == null) return;
            organization.IsDeleted = true;
            await _db.SaveChangesAsync();
        }

        // получение всех организаций
        public async Task<IEnumerable<OrganizationDto>> GetAllAsync()
        {
            return await _db.Organizations.Where(o => !o.IsDeleted).ToListAsync();
        }

        // получение организации по id
        public async Task<OrganizationDto> GetByIdAsync(int id)
        {
            return await _db.Organizations.FirstOrDefaultAsync(o => o.Id == id && !o.IsDeleted);
        }

        // получение всех счетов организации
        public async Task<IEnumerable<AccountDto>> GetByOrganizationIdAsync(int organizationtId)
        {
            return await _db.Accounts.Where(a => a.OrganizationId == organizationtId).ToListAsync();
        }

        // изменение организации
        public async Task UpdateAsync(OrganizationDto organization)
        {
            _db.Organizations.Update(organization);
            await _db.SaveChangesAsync();
        }
    }
}
