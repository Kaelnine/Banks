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
    public class BankRepository : IBankRepository
    {
        private readonly BankDbContext _db;
        public BankRepository(BankDbContext db) {  _db = db; }

        // добавление банка
        public async Task AddAsync(BankDto bank)
        {
            await _db.Banks.AddAsync(bank);
            await _db.SaveChangesAsync();
        }

        // удаление банка
        public async Task DeleteAsync(int id)
        {
            var bank = await _db.Banks.FindAsync(id);
            if (bank == null) return;
            bank.IsDeleted = true;
            await _db.SaveChangesAsync();
        }

        // получение всех банков
        public async Task<IEnumerable<BankDto>> GetAllAsync()
        {
            return await _db.Banks.Where(b => !b.IsDeleted).ToListAsync();
        }

        // получение банка по id
        public async Task<BankDto> GetByIdAsync(int id)
        {
            return await _db.Banks.FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);
        }

        // изменение банка
        public async Task UpdateAsync(BankDto bank)
        {
            _db.Banks.Update(bank);
            await _db.SaveChangesAsync();
        }        
    }
}
