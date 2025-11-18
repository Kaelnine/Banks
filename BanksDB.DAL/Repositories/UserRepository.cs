using AutoMapper;
using BanksDB.Core.Data;
using BanksDB.Core.Dtos;
using BanksDB.Core.Entities;
using BanksDB.Core.Interfaces;
using BanksDB.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BanksDB.DAL.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbContextFactory<BankDbContext> _db;
        private readonly IMapper _mapper;
        public UserRepository(IDbContextFactory<BankDbContext> db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task AddAsync(User user)
        {
            await using var db = await _db.CreateDbContextAsync();            
            await db.Users.AddAsync(user);
            await db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int userId)
        {
            await using var db = await _db.CreateDbContextAsync();            
            var user = await db.Users.FindAsync(userId);
            if (user == null) return;
            user.IsDeleted = true;
            await db.SaveChangesAsync();
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            await using var db = await _db.CreateDbContextAsync();            
            var users = await db.Users.Where(b => !b.IsDeleted).ToListAsync();
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task<UserDto> GetByIdAsync(int id)
        {
            await using var db = await _db.CreateDbContextAsync();            
            var user = await db.Users.FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
            if (user == null) return null;
            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                PasswordHash = user.PasswordHash,
                FullName = user.FullName,
                Role = user.Role
            };
        }

        public async Task<UserDto?> GetByNameAsync(string name)
        {
            await using var db = await _db.CreateDbContextAsync();
            var user = await db.Users.FirstOrDefaultAsync(u => u.UserName == name && !u.IsDeleted);
            if (user == null) return null;
            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                PasswordHash = user.PasswordHash,
                FullName = user.FullName,
                Role = user.Role
            };
        }

        public async Task UpdateAsync(User user)
        {
            await using var db = await _db.CreateDbContextAsync();            
            db.Users.Update(user);
            await db.SaveChangesAsync();
        }
    }
}
