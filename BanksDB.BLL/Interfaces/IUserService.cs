using BanksDB.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BanksDB.BLL.Interfaces
{
    public interface IUserService
    {
        Task<UserDto?> ValidateUserAsync(string username, string password);        
    }
}
