using BanksDB.Core.Dtos;

namespace BanksDB.BLL.Interfaces
{
    public interface IUserService
    {
        Task<UserDto?> ValidateUserAsync(string username, string password);
    }
}
