using AutoMapper;
using BanksDB.BLL.Interfaces;
using BanksDB.Core.Entities;
using BanksDB.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BanksDB.BLL.Services
{
    public class BankService : IBankService
    {
        private readonly IBankRepository _bankRepository;
        private readonly IMapper _mapper;
        public BankService(IBankRepository bankRepository, IMapper mapper)
        {
            _bankRepository = bankRepository;
            _mapper = mapper;
        }

        public async Task AddBankAsync(Bank bank)
        {
            await _bankRepository.AddAsync(bank);
        }

        public async Task DeleteBankAsync(int id)
        {
            await _bankRepository.DeleteAsync(id);
        }

        public async Task<List<Bank>> GetAllBanksAsync()
        {
            var banks =  await _bankRepository.GetAllAsync();
            return _mapper.Map<List<Bank>>(banks);
        }

        public async Task<Bank> GetBankByIdAsync(int id)
        {
            var bank = await _bankRepository.GetByIdAsync(id);
            return _mapper.Map<Bank>(bank);
        }

        public async Task UpdateBankAsync(Bank bank)
        {
            await _bankRepository.UpdateAsync(bank);
        }
    }
}
