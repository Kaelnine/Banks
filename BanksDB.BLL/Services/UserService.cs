using BanksDB.BLL.Interfaces;
using BanksDB.BLL.Security;
using BanksDB.Core.Dtos;
using BanksDB.Core.Interfaces;
using BanksDB.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BanksDB.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserDto?> ValidateUserAsync(string username, string password)
        {
            var user = await _userRepository.GetByNameAsync(username);
            if (user == null) return null;
            bool isValid = PasswordHasher.VerifyPassword(password, user.PasswordHash);
            return isValid ? user : null;
        }
    }
}
