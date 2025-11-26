using AutoMapper;
using BanksDB.DAL.Data;
using BanksDB.Core.Dtos;
using BanksDB.Core.Entities;
using BanksDB.Core.Interfaces;
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
        //private readonly BankDbContext _db;
        private readonly IDbContextFactory<BankDbContext> _db;
        private readonly IMapper _mapper;
        public OrganizationRepository(IDbContextFactory<BankDbContext> db, IMapper mapper) 
        { 
            _db = db;
            _mapper = mapper;
        }

        // добавление организации
        public async Task AddAsync(Organization organization)
        {
            await using var db = await _db.CreateDbContextAsync();
            //await _db.Organizations.AddAsync(organization);
            //await _db.SaveChangesAsync();
            await db.Organizations.AddAsync(organization);
            await db.SaveChangesAsync();
        }

        // удаление организации
        public async Task DeleteAsync(int id)
        {
            await using var db = await _db.CreateDbContextAsync();
            //var organization = await _db.Organizations.FindAsync(id);
            //if (organization == null) return;
            //organization.IsDeleted = true;
            //await _db.SaveChangesAsync();
            var organization = await db.Organizations.FindAsync(id);
            if (organization == null) return;
            organization.IsDeleted = true;
            await db.SaveChangesAsync();
        }

        // получение всех организаций
        public async Task<IEnumerable<OrganizationDto>> GetAllAsync()
        {
            await using var db = await _db.CreateDbContextAsync();
            //return await _db.Organizations.Where(o => !o.IsDeleted).ToListAsync();
            var organizations = await db.Organizations.Where(o => !o.IsDeleted).ToListAsync();
            return _mapper.Map<IEnumerable<OrganizationDto>>(organizations);
        }

        // получение организации по id
        public async Task<OrganizationDto> GetByIdAsync(int id)
        {
            await using var db = await _db.CreateDbContextAsync();
            //return await _db.Organizations.FirstOrDefaultAsync(o => o.Id == id && !o.IsDeleted);
            var organization = await db.Organizations
                .Include(o => o.Accounts)
                .FirstOrDefaultAsync(o => o.Id == id && !o.IsDeleted);
            if (organization == null) return null;
            return new OrganizationDto
            {
                Id = id,
                Name = organization.Name,
                Inn = organization.Inn,
                Kpp = organization.Kpp,
                Address = organization.Address,
                Accounts = organization.Accounts
                .Where(a => !a.IsDeleted)
                .Select(a => new AccountDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    OrganizationId = a.OrganizationId,
                    BankId = a.BankId,
                    BankName = a.Bank.Name,
                    AccountNumber = a.AccountNumber,
                    CurrentBalance = a.CurrentBalance,
                    UpdateAccount = a.UpdateAccount
                }).ToList()
            };
        }

        // получение всех счетов организации
        public async Task<IEnumerable<AccountDto>> GetByOrganizationIdAsync(int organizationtId)
        {
            await using var db = await _db.CreateDbContextAsync();
            //return await _db.Accounts.Where(a => a.OrganizationId == organizationtId).ToListAsync();
            var accounts = await db.Accounts
                .Include(a => a.Bank)
                .Include(a => a.Organization)
                .Where(a => !a.IsDeleted)
                .ToListAsync();
            return _mapper.Map<IEnumerable<AccountDto>>(accounts);
        }

        // изменение организации
        public async Task UpdateAsync(Organization organization)
        {
            await using var db = await _db.CreateDbContextAsync();
            //_db.Organizations.Update(organization);
            //await _db.SaveChangesAsync();
            db.Organizations.Update(organization);
            await db.SaveChangesAsync();
        }
    }
}
