using BanksDB.Core.Dtos;
using BanksDB.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BanksDB.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<UserDto>> GetAllAsync();// получение всех пользователей
        Task<UserDto> GetByIdAsync(int id);// получение пользователя по id
        Task<UserDto?> GetByNameAsync(string name); // получение пользователя по имени
        Task AddAsync(User user);// создание пользователя 
        Task UpdateAsync(User user);// изменение пользователя 
        Task DeleteAsync(int userId);// удаление пользователя
    }
}
