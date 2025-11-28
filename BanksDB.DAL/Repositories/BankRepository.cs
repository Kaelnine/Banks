using AutoMapper;
using BanksDB.Core.Dtos;
using BanksDB.Core.Entities;
using BanksDB.Core.Interfaces;
using BanksDB.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace DBBanks.DAL.Repositories
{
    public class BankRepository : IBankRepository
    {
        private readonly IDbContextFactory<BankDbContext> _db;
        private readonly IMapper _mapper;
        public BankRepository(IDbContextFactory<BankDbContext> db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        // добавление банка
        public async Task AddAsync(Bank bank)
        {
            await using var db = await _db.CreateDbContextAsync();
            await db.Banks.AddAsync(bank);
            await db.SaveChangesAsync();
        }

        // удаление банка
        public async Task DeleteAsync(int id)
        {
            await using var db = await _db.CreateDbContextAsync();
            var bank = await db.Banks.FindAsync(id);
            if (bank == null) return;
            bank.IsDeleted = true;
            await db.SaveChangesAsync();
        }

        // получение всех банков
        public async Task<IEnumerable<BankDto>> GetAllAsync()
        {
            await using var db = await _db.CreateDbContextAsync();
            var banks = await db.Banks.Where(b => !b.IsDeleted).ToListAsync();
            return _mapper.Map<IEnumerable<BankDto>>(banks);
        }

        // получение банка по id
        public async Task<BankDto> GetByIdAsync(int id)
        {
            await using var db = await _db.CreateDbContextAsync();
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
        public async Task UpdateAsync(Bank bank)
        {
            await using var db = await _db.CreateDbContextAsync();
            db.Banks.Update(bank);
            await db.SaveChangesAsync();
        }
    }
}
