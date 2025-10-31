using BanksDB.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BanksDB.Core.Interfaces
{
    public interface IBankRepository
    {
        Task<IEnumerable<BankDto>> GetAllAsync();// получение всех банков
        Task <BankDto>GetByIdAsync(int id);// получение банка по id                
        Task AddAsync(BankDto bank);// создание банка
        Task UpdateAsync(BankDto bank);// изменение банка
        Task DeleteAsync(int bankId);// удаление банка
    }
}
