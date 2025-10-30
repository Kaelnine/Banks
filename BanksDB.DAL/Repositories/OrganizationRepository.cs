using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBBanks.DAL.Repositories
{
    public class OrganizationRepository : IRepository<OrganizationDto>
    {
        private readonly BankDbContext _db;
        public OrganizationRepository(BankDbContext db) { _db = db; }

        public async Task AddAsync(OrganizationDto organization)
        {
            await _db.Organizations.AddAsync(organization);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var organization = await _db.Organizations.FindAsync(id);
            if (organization == null) return;
            organization.IsDeleted = true;
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<OrganizationDto>> GetAllAsync()
        {
            return await _db.Organizations.Where(o => !o.IsDeleted).ToListAsync();
        }

        public async Task<OrganizationDto> GetByIdAsync(int id)
        {
            return await _db.Organizations.FirstOrDefaultAsync(o => o.Id == id && !o.IsDeleted);
        }

        public async Task UpdateAsync(OrganizationDto organization)
        {
            _db.Organizations.Update(organization);
            await _db.SaveChangesAsync();
        }
    }
}
