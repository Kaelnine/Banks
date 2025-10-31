using AutoMapper;
using BanksDB.BLL.Interfaces;
using BanksDB.Core.Dtos;
using BanksDB.Core.Interfaces;
using BanksDB.Core.Models.InputModels;
using BanksDB.Core.Models.OutputModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BanksDB.BLL.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _mapper;

        public AccountService(IAccountRepository accountRepository, IMapper mapper)
        {
            _accountRepository = accountRepository;
            _mapper = mapper;
        }
        // добавление счета
        public async Task AddAsync(AccountInputModel inputModel)
        {
            var accountDto = _mapper.Map<AccountDto>(inputModel);
            await _accountRepository.AddAsync(accountDto);            
        }

        public Task<bool> DeleteAsync(int id)
        {
            
        }

        public Task<List<AccountOutputModel>> GetAccountSummaryAsync()
        {
            
        }

        public Task<List<AccountOutputModel>> GetAllAsync()
        {
            
        }

        public Task<AccountOutputModel> GetByIdAsync(int id)
        {
            
        }

        public Task<List<AccountOutputModel>> GetByOrganizationIdAsync(int organizationId)
        {
            
        }

        public Task<AccountOutputModel> UpdateAsync(int id, AccountInputModel inputModel)
        {
            
        }
    }
}
