using AutoMapper;
using BanksDB.BLL.Interfaces;
using BanksDB.BLL.Parsers;
using BanksDB.Core.Dtos;
using BanksDB.Core.Enums;
using BanksDB.Core.Interfaces;
using BanksDB.Core.Models.InputModels;
using BanksDB.Core.Models.OutputModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        private readonly BankParser _bankParser;
        public TransactionService(ITransactionRepository transactionRepository, IAccountRepository accountRepository, IMapper mapper, BankParser bankParser)
        {
            _transactionRepository = transactionRepository;
            _accountRepository = accountRepository;
            _mapper = mapper;
            _bankParser = bankParser;
        }

        public async Task<BankParserResult> BankParserStatementAsync(Stream fileStream, int accountId)
        {
            var account = await _accountRepository.GetByIdAsync(accountId);
            var accountNumber = account?.AccountNumber;
            var result = _bankParser.ParseFile(fileStream, accountNumber);
            foreach (var transaction in result.Transactions)
            {
                transaction.AccountId = accountId;
            }
            return result;
        }

        public async Task<List<TransactionOutputModel>> ImportTransactionsAsync(List<TransactionInputModel> transactions)
        {
            if (!transactions.Any())
            {
                throw new ArgumentException("Список транзакций пуст");
            }
            var validationResult = ValidateTransactions(transactions);
            if (!validationResult.IsValid)
            {
                throw new ArgumentException($"Ошибки валидации: {string.Join("; ", validationResult.Errors)}");
            }
            var transactionsDto = _mapper.Map<List<TransactionDto>>(transactions);
            var createdTransactions = await _transactionRepository.AddSeveralAsync(transactionsDto);
            await UpdateAccountBalances(transactions);
            return _mapper.Map<List<TransactionOutputModel>>(createdTransactions);
        }

        private async Task UpdateAccountBalances(List<TransactionInputModel> transactions)
        {
            var accountGroups = transactions.GroupBy(t => t.AccountId);
            foreach (var group in accountGroups)
            {
                var account = await _accountRepository.GetByIdAsync(group.Key);
                if (account == null)
                {
                    continue;
                }
                var balanceChange = group.Sum(t => t.TransactionType == TransactionType.Приход.ToString() ? t.Amount : -t.Amount);
                account.CurrentBalance += balanceChange;
                account.UpdateAccount = DateTime.Now;
                await _accountRepository.UpdateAsync(account);
            }
        }

        private ValidationResult ValidateTransactions(List<TransactionInputModel> transactions)
        {
            var result = new ValidationResult
            {
                IsValid = true,
                Errors = new List<string>()
            };
            for (int i = 0; i < transactions.Count; i++)
            {
                var transaction = transactions[i];
                if (transaction.Amount <= 0)
                {
                    result.Errors.Add($"Транзакция {i + 1}: Сумма должна быть больше 0");
                }
                if (transaction.TransactionDate > DateTime.Now)
                {
                    result.Errors.Add($"Транзакция {i + 1}: Дата не может быть в будущем");
                }
                if (string.IsNullOrEmpty(transaction.TransactionType))
                {
                    result.Errors.Add($"Транзакция {i + 1}: Не указан тип транзакции");
                }
                if (transaction.TransactionType != "Приход" && transaction.TransactionType != "Расход")
                {
                    result.Errors.Add($"Транзакция {i + 1}: Тип транзакции должен быть 'Приход' или 'Расход'");
                }                       
            }
            result.IsValid = !result.Errors.Any();
            return result;
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

    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
