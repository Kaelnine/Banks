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

        public async Task AddTransactionAsync(TransactionInputModel inputModel)
        {
            var account = await _accountRepository.GetByIdAsync(inputModel.AccountId);
            if (account == null)
            {
                throw new ArgumentException($"Счет {account.Name} не найден");
            }
            if (inputModel.Amount <= 0)
            {
                throw new ArgumentException("Сумма транзакции должна быть больше 0");
            }
            var transactionDto = _mapper.Map<TransactionDto>(inputModel);
            await _transactionRepository.AddAsync(transactionDto);
            //await UpdateAccountBalance(account, inputModel.Amount, inputModel.TransactionType);            
        }

        public async Task DeleteTransactionAsync(int id)
        {
            await _transactionRepository.DeleteAsync(id);
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

        public async Task<TransactionOutputModel> GetTransactionByIdAsync(int id)
        {
            var transaction = await _transactionRepository.GetByIdAsync(id);
            return _mapper.Map<TransactionOutputModel>(transaction);
        }

        public async Task<List<TransactionOutputModel>> GetTransactionsByAccountAndDateAsync(int accountId, DateTime date)
        {
            var transactions = await _transactionRepository.GetByAccountIdForDayAsync(accountId, date);
            return _mapper.Map<List<TransactionOutputModel>>(transactions);
        }

        public async Task<List<TransactionOutputModel>> GetTransactionsByAccountAndPeriodAsync(int accountId, DateTime startDate, DateTime endDate)
        {
            var transactions = await _transactionRepository.GetByAccountIdForPeriodAsync(accountId, startDate, endDate);
            return _mapper.Map<List<TransactionOutputModel>>(transactions);
        }

        public async Task<List<TransactionOutputModel>> GetTransactionsByAccountAsync(int accountId)
        {
            var transactions = await _transactionRepository.GetByAccountIdAsync(accountId);
            return _mapper.Map<List<TransactionOutputModel>>(transactions);
        }

        public async Task<TransactionOutputModel> UpdateTransactionAsync(int id, TransactionInputModel inputModel)
        {
            var transaction = await _transactionRepository.GetByIdAsync(id);
            if (transaction == null)
            {
                throw new ArgumentException($"Транзакция с номером {id} не найдена");
            }
            //  здесь надо реадизовать изменение баланса при установки у транзакции isDeleted = true, наверно лучше сделать на уровне бд
            var account = await _accountRepository.GetByIdAsync(transaction.AccountId);
            return _mapper.Map<TransactionOutputModel>(transaction);
        }

        //private async Task UpdateAccountBalance()
    }
}
