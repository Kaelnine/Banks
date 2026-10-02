using AutoMapper;
using BanksDB.BLL.Interfaces;
using BanksDB.BLL.Parsers;
using BanksDB.Core.Dtos;

using BanksDB.Core.Entities;
using BanksDB.Core.Enums;
using BanksDB.Core.Interfaces;
using BanksDB.Core.Models.InputModels;
using BanksDB.Core.Models.OutputModels;
using BanksDB.DAL.Data;
using Microsoft.EntityFrameworkCore;


namespace BanksDB.BLL.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IMapper _mapper;
        private readonly IAccountRepository _accountRepository;
        private readonly BankParser _bankParser;
        private readonly ICategoryClassifier _classifier;
        private readonly IDbContextFactory<BankDbContext> _db;
        public TransactionService(ITransactionRepository transactionRepository, IAccountRepository accountRepository, IMapper mapper, BankParser bankParser, ICategoryClassifier classifier, IDbContextFactory<BankDbContext> db)
        {
            _transactionRepository = transactionRepository;
            _accountRepository = accountRepository;
            _mapper = mapper;
            _bankParser = bankParser;
            _classifier = classifier;
            _db = db;
        }

        public async Task<BankParserResult> BankParserStatementAsync(Stream fileStream, int accountId)
        {
            var account = await _accountRepository.GetByIdAsync(accountId);
            var accountNumber = account?.AccountNumber;
            using var memoryStream = new MemoryStream();
            await fileStream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;
            var result = _bankParser.ParseFile(memoryStream, accountNumber);
            foreach (var transaction in result.Transactions)
            {
                transaction.AccountId = accountId;
            }
            return result;
        }

        public async Task<List<Transaction>> ImportTransactionsAsync(List<TransactionInputModel> transactions)
        {
            if (!transactions.Any())
            {
                return new List<Transaction>();
            }
            
            var validationResult = ValidateTransactions(transactions);
            if (!validationResult.IsValid)
            {
                throw new ArgumentException($"Ошибки валидации: {string.Join("; ", validationResult.Errors)}");
            }
            var uniqueTransactions = await FilterDuplicateTransactionsAsync(transactions);
            if (!uniqueTransactions.Any())
            {
                throw new InvalidOperationException("Весь список транзакций уже существует");
            }

            var transactionsEntity = _mapper.Map<List<Transaction>>(uniqueTransactions);
            var createdTransactions = await _transactionRepository.AddSeveralAsync(transactionsEntity);
            //await UpdateAccountBalances(transactions);
            
            return _mapper.Map<List<Transaction>>(createdTransactions);
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
                var accountEntity = _mapper.Map<Account>(account);
                await _accountRepository.UpdateAsync(accountEntity);
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
                var accountEntity = _mapper.Map<Account>(account);
                await _accountRepository.UpdateAsync(accountEntity);
            }
            var transactionsEntity = _mapper.Map<List<Transaction>>(inputModels);
            var createdTransactions = await _transactionRepository.AddSeveralAsync(transactionsEntity);
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
            var transactionEntity = _mapper.Map<Transaction>(inputModel);
            await _transactionRepository.AddAsync(transactionEntity);
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

        public async Task<List<Transaction>> GetTransactionsByAccountAndDateAsync(int accountId, DateTime date)
        {
            var transactions = await _transactionRepository.GetByAccountIdForDayAsync(accountId, date);
            return _mapper.Map<List<Transaction>>(transactions);
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

        public async Task<bool> IsDuplicateTransactionAsync(TransactionInputModel transaction)
        {
            return await _transactionRepository.IsDuplicateTransactionAsync(transaction);
        }

        public async Task<List<TransactionInputModel>> FilterDuplicateTransactionsAsync(List<TransactionInputModel> transactions)
        {
            return await _transactionRepository.FilterDuplicateTransactionsAsync(transactions);
        }

        public async Task<int> GetDuplicateCountAsync(List<TransactionInputModel> transactions)
        {
            return await _transactionRepository.GetDuplicateCountAsync(transactions);
        }

        public async Task<PivotMonthDto> GetMonthlyPivotAsync(int year, int month, string? categoryFilter = null, string? transactionTypeFilter = null)
        {
            await using var db = await _db.CreateDbContextAsync();

            var start = new DateTime(year, month, 1);
            var end = start.AddMonths(1);
            var daysInMonth = DateTime.DaysInMonth(year, month);

            // Тянем только нужные поля + организацию через Account.Organization.
            var raw = await db.Transactions
                .Where(t => !t.IsDeleted
                            && t.TransactionDate >= start
                            && t.TransactionDate < end
                            && !t.Account.IsDeleted)
                .Select(t => new
                {
                    t.TransactionDate,
                    t.Amount,
                    t.TransactionType,
                    t.Description,
                    t.DisplayCounterparty,
                    OrgId = t.Account.OrganizationId,
                    OrgName = t.Account.Organization.Name
                })
                .ToListAsync();

            // Классифицируем в памяти (нельзя делать в SQL).
            var classified = raw
                .Select(t => new
                {
                    t.TransactionDate,
                    t.Amount,
                    t.TransactionType,
                    Category = _classifier.Classify(t.Description, t.TransactionType, t.DisplayCounterparty),
                    t.OrgId,
                    t.OrgName
                })
                .Where(t => string.IsNullOrEmpty(categoryFilter) || t.Category == categoryFilter)
                .Where(t => string.IsNullOrEmpty(transactionTypeFilter) || t.TransactionType == transactionTypeFilter)
                .ToList();

            var result = new PivotMonthDto
            {
                Year = year,
                Month = month,
                DaysInMonth = daysInMonth,
                DayTotals = Enumerable.Range(0, daysInMonth).Select(_ => new PivotCellDto()).ToList()
            };

            foreach (var group in classified.GroupBy(t => new { t.Category, t.TransactionType }))
            {
                var row = new PivotRowDto
                {
                    Category = group.Key.Category,
                    TransactionType = group.Key.TransactionType,
                    CellsByDay = Enumerable.Range(0, daysInMonth).Select(_ => new PivotCellDto()).ToList()
                };

                foreach (var dayGroup in group.GroupBy(t => t.TransactionDate.Day))
                {
                    var idx = dayGroup.Key - 1;
                    var cell = row.CellsByDay[idx];
                    cell.Total = dayGroup.Sum(t => t.Amount);
                    cell.Organizations = dayGroup
                        .GroupBy(t => new { t.OrgId, t.OrgName })
                        .Select(og => new OrganizationAmountDto
                        {
                            OrganizationId = og.Key.OrgId,
                            OrganizationName = og.Key.OrgName,
                            Amount = og.Sum(x => x.Amount),
                            TransactionsCount = og.Count()
                        })
                        .OrderByDescending(o => o.Amount)
                        .ToList();

                    // В итог дня кладём ту же ячейку — так модалка итога тоже покажет разбивку.
                    result.DayTotals[idx].Total += cell.Total;
                    result.DayTotals[idx].Organizations.AddRange(cell.Organizations);
                }

                row.MonthTotal = row.CellsByDay.Sum(c => c.Total);
                result.Rows.Add(row);

                if (group.Key.TransactionType == "Приход") result.TotalIncome += row.MonthTotal;
                else result.TotalExpense += row.MonthTotal;
            }

            result.Rows = result.Rows
                .OrderBy(r => r.TransactionType == "Расход" ? 0 : 1) // сначала расходы
                .ThenByDescending(r => r.MonthTotal)
                .ToList();

            // Пересобираем итоги дня после возможной консолидации (в DayTotals уже накоплено).
            foreach (var dayCell in result.DayTotals)
            {
                dayCell.Organizations = dayCell.Organizations
                    .GroupBy(o => new { o.OrganizationId, o.OrganizationName })
                    .Select(g => new OrganizationAmountDto
                    {
                        OrganizationId = g.Key.OrganizationId,
                        OrganizationName = g.Key.OrganizationName,
                        Amount = g.Sum(x => x.Amount),
                        TransactionsCount = g.Sum(x => x.TransactionsCount)
                    })
                    .OrderByDescending(o => o.Amount)
                    .ToList();
            }

            return result;
        }
    }

    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new();
    }


}
