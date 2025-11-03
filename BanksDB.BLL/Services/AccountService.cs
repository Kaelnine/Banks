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

        // удаление счета
        public async Task DeleteAsync(int id)
        {
            await _accountRepository.DeleteAsync(id);
        }

        // получение сводки по счетам
        public async Task<List<AccountOutputModel>> GetAccountSummaryAsync()
        {
            var summary = await _accountRepository.GetAccountSummaryAsync();
            return _mapper.Map<List<AccountOutputModel>>(summary);
        }

        // получение всех счетов
        public async Task<List<AccountOutputModel>> GetAllAsync()
        {
            var accounts = await _accountRepository.GetAllAsync();
            return _mapper.Map<List<AccountOutputModel>>(accounts);
        }

        // получение счета по id
        public async Task<AccountOutputModel> GetByIdAsync(int id)
        {
            var account = await _accountRepository.GetByIdAsync(id);
            return _mapper.Map<AccountOutputModel>(account);
        }

        // получение всех счетов организации
        public async Task<List<AccountOutputModel>> GetByOrganizationIdAsync(int organizationId)
        {
            var acounts = await _accountRepository.GetByOrganizationIdAsync(organizationId);
            return _mapper.Map<List<AccountOutputModel>>(acounts);
        }

        // обновление счета
        public async Task UpdateAsync(int id, AccountInputModel inputModel)
        {
            var accountDto = _mapper.Map<AccountDto>(inputModel);
            accountDto.Id = id;
            await _accountRepository.UpdateAsync(accountDto);            
        }
    }
}
