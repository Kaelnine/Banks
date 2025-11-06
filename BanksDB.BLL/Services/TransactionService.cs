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
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IMapper _mapper;
        private readonly IAccountRepository _accountRepository;
        public TransactionService(ITransactionRepository transactionRepository, IAccountRepository accountRepository, IMapper mapper)
        {
            _transactionRepository = transactionRepository;
            _accountRepository = accountRepository;
            _mapper = mapper;
        }

        public async Task<List<TransactionOutputModel>> AddSeveralTransactionsAsync(List<TransactionInputModel> inputModels)
        {
            if (!inputModels.Any())
            {
                throw new ArgumentException("Список транзакций  пуст");
            }
            var accountGroup = inputModels.GroupBy(t => t.AccountId);
            foreach (var group in accountGroup)
            {
                var account = await _accountRepository.GetByIdAsync(group.Key);
                if (account == null)
                {
                    throw new ArgumentException($"Счет с ID {group.Key} не найден");
                }
                var totalChange = group.Sum(t => t.TransactionType == "Приход" ? t.Amount : -t.Amount);
                account.CurrentBalance += totalChange;
                await _accountRepository.UpdateAsync(account);
            }
                var transactionsDto = _mapper.Map<List<TransactionDto>>(inputModels);
                var createdTransactions = await _transactionRepository.AddSeveralAsync(transactionsDto);
                return _mapper.Map<List<TransactionOutputModel>>(createdTransactions);            
        }

        public Task<TransactionOutputModel> AddTransactionAsync(TransactionInputModel inputModel)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteTransactionAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<TransactionOutputModel>> GetAllTransactionsAsync()
        {
            var transactions = await _transactionRepository.GetAllAsync();
            return _mapper.Map<List<TransactionOutputModel>>(transactions);
        }

        public async Task<List<DailySummaryOutputModel>> GetDailySummaryAsync(int accountId, DateTime startDate, DateTime endDate)
        {
            var summary = await _transactionRepository.GetDailySummaryAsync(accountId, startDate, endDate);
            return _mapper.Map<List<DailySummaryOutputModel>>(summary);
        }

        public Task<TransactionOutputModel> GetTransactionByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<TransactionOutputModel>> GetTransactionsByAccountAndDateAsync(int accountId, DateTime date)
        {
            throw new NotImplementedException();
        }

        public Task<List<TransactionOutputModel>> GetTransactionsByAccountAndPeriodAsync(int accountId, DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public Task<List<TransactionOutputModel>> GetTransactionsByAccountAsync(int accountId)
        {
            throw new NotImplementedException();
        }

        public Task<TransactionOutputModel> UpdateTransactionAsync(int id, TransactionInputModel inputModel)
        {
            throw new NotImplementedException();
        }
    }
}
