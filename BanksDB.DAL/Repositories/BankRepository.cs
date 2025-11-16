using AutoMapper;
using BanksDB.Core.Data;
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
    public class BankRepository : IBankRepository
    {
        //private readonly BankDbContext _db;
        private readonly IDbContextFactory<BankDbContext> _db;
        private readonly IMapper _mapper;
        public BankRepository(IDbContextFactory<BankDbContext> db, IMapper mapper) 
        {  
            _db = db;
            _mapper = mapper;
        }

        // добавление банка
        public async Task AddAsync(Bank bank) // BankDto
        {
            await using var db = await _db.CreateDbContextAsync();
            //await _db.Banks.AddAsync(bank);
            //await _db.SaveChangesAsync();
            await db.Banks.AddAsync(bank);
            await db.SaveChangesAsync();
        }

        // удаление банка
        public async Task DeleteAsync(int id)
        {
            await using var db = await _db.CreateDbContextAsync();
            //var bank = await _db.Banks.FindAsync(id);
            //if (bank == null) return;
            //bank.IsDeleted = true;
            //await _db.SaveChangesAsync();
            var bank = await db.Banks.FindAsync(id);
            if (bank == null) return;
            bank.IsDeleted = true;
            await db.SaveChangesAsync();
        }

        // получение всех банков
        public async Task<IEnumerable<BankDto>> GetAllAsync() // BankDto
        {
            await using var db = await _db.CreateDbContextAsync();
            //return await _db.Banks.Where(b => !b.IsDeleted).ToListAsync();
            var banks = await db.Banks.Where(b  => !b.IsDeleted).ToListAsync();
            return _mapper.Map<IEnumerable<BankDto>>(banks);            
        }

        // получение банка по id
        public async Task<BankDto> GetByIdAsync(int id) // BankDto
        {
            await using var db = await _db.CreateDbContextAsync();
            //return await _db.Banks.FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);
            var bank = await db.Banks.FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);
            if (bank == null) return null;
            return new BankDto
            {
                Id = bank.Id,
                Name = bank.Name,
                Bik = bank.Bik,
                CorrespondentAccount = bank.CorrespondentAccount,
                Address = bank.Address
            };
        }

        // изменение банка
        public async Task UpdateAsync(Bank bank) // BankDto
        {
            await using var db = await _db.CreateDbContextAsync();
            //_db.Banks.Update(bank);
            //await _db.SaveChangesAsync();
            db.Banks.Update(bank);
            await db.SaveChangesAsync();
        }        
    }
}
