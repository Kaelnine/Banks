using BanksDB.Core.Dtos;
using BanksDB.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BanksDB.Core.Interfaces
{
    public interface IBankRepository
    {
        Task<IEnumerable<BankDto>> GetAllAsync();// получение всех банков // BankDto
        Task <BankDto>GetByIdAsync(int id);// получение банка по id         // BankDto        
        Task AddAsync(Bank bank);// создание банка // BankDto
        Task UpdateAsync(Bank bank);// изменение банка // BankDto
        Task DeleteAsync(int bankId);// удаление банка 
    }
}
