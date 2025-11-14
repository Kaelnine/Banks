using BanksDB.Core.Enums;
using BanksDB.Core.Models.InputModels;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BanksDB.BLL.Parsers
{
    public class BankParser
    {
        public BankParserResult ParseFile(Stream fileStream, string expectedAccountNumber = null)
        {
            var transactions = new List<TransactionInputModel>();
            var errors = new List<string>();
            using var reader = new StreamReader(fileStream, Encoding.GetEncoding(1251));
            string line;
            var inDocumentSection = false;
            TransactionInputModel currentTransaction = null;
            var lineNumber = 0;
            string fileAccountNumber = null;
            while ((line = reader.ReadLine()) != null)
            {
                lineNumber++;
                line = line.Trim();
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }
                try
                {
                    if (line.StartsWith("РасчСчет=") && fileAccountNumber == null)
                    {
                        fileAccountNumber = line.Split('=')[1].Trim();
                    }
                    if (line.StartsWith("СекцияДокумент=Платежное поручение"))
                    {
                        inDocumentSection = true;
                        currentTransaction = new TransactionInputModel();
                        continue;
                    }
                    if (line == "КонецДокумента" && inDocumentSection)
                    {
                        inDocumentSection = false;
                        if (IsValidTransaction(currentTransaction))
                        {
                            DetermineTransactionType(currentTransaction);
                            transactions.Add(currentTransaction);
                        }
                        currentTransaction = null;
                        continue;
                    }
                    if (inDocumentSection && currentTransaction != null)
                    {
                        ParseDocumentLine(line, currentTransaction);
                    }
                }
                catch (Exception ex)
                {
                    errors.Add($"Строка {lineNumber}: {ex.Message}");
                }
            }
            if (!string.IsNullOrEmpty(expectedAccountNumber) && !string.IsNullOrEmpty(fileAccountNumber) && fileAccountNumber != expectedAccountNumber)
            {
                errors.Add($"Номер счета в файле ({fileAccountNumber}) не соответствует выбранному номеру счета ({expectedAccountNumber})");
            }
            return new BankParserResult
            {
                Transactions = transactions,
                Errors = errors,
                IsValid = !errors.Any(),
                FileAccountNumber = fileAccountNumber
                //TotalTransactionsFound = transactions.Count
            };
        }
        private void ParseDocumentLine(string line, TransactionInputModel currentTransaction)
        {
            var parts = line.Split('=', 2);
            if (parts.Length != 2)
            {
                return;
            }
            var key = parts[0].Trim();
            var value = parts[1].Trim();
            switch (key)
            {
                case "Дата":
                    if (DateTime.TryParseExact(value, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                    {
                        currentTransaction.TransactionDate = date;                        
                    }
                    break;
                case "Сумма":
                    if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var amount))
                    {
                        currentTransaction.Amount = amount;
                    }
                    break;
                case "НазначениеПлатежа":
                    currentTransaction.Description = value;
                    break;
                case "Получатель":
                    currentTransaction.CounterpartyName = value;
                    break;
                case "ПолучательРасчСчет":
                    if (!string.IsNullOrEmpty(value))
                    {
                        currentTransaction.CounterpartyAccount = value;
                    }
                    break;
                case "ПолучательИНН":
                    if (!string.IsNullOrEmpty(value))
                    {
                        currentTransaction.CounterpartyInn = value;
                    }                        
                    break;
                case "Номер":
                    currentTransaction.DocumentNumber = value;
                    break;
                case "Плательщик":
                    currentTransaction.PayerName = value;
                    break;
                case "ПлательщикИНН":
                    currentTransaction.PayerInn = value;
                    break;
                case "ПлательщикРасчСчет":
                    currentTransaction.PayerAccount = value;
                    break;
                case "ДатаСписано":
                    if (DateTime.TryParseExact(value, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var writeOffDate))
                    {
                        currentTransaction.WriteOffDate = writeOffDate;
                    }                        
                    break;
                case "ДатаПоступило":
                    if (DateTime.TryParseExact(value, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var receiptDate))
                    {
                        currentTransaction.ReceiptDate = receiptDate;
                    }                        
                    break;
            }
        }
        private void DetermineTransactionType(TransactionInputModel currentTransaction)
        {
            if (!string.IsNullOrEmpty(currentTransaction.CounterpartyName))
            {
                if (currentTransaction.CounterpartyName.Contains("Сбербанк") || currentTransaction.CounterpartyName.Contains("Банк"))
                {
                    currentTransaction.TransactionType = TransactionType.Расход.ToString();
                }
                else
                {
                    if (currentTransaction.WriteOffDate != default && currentTransaction.ReceiptDate == default)
                    {
                        currentTransaction.TransactionType = TransactionType.Расход.ToString();
                    }
                    else if (currentTransaction.ReceiptDate != default && currentTransaction.WriteOffDate == default)
                    {
                        currentTransaction.TransactionType = TransactionType.Приход.ToString();
                    }
                    else
                    {
                        currentTransaction.TransactionType = TransactionType.Расход.ToString();
                    }
                }
            }
        }
        private bool IsValidTransaction(TransactionInputModel currentTransaction)
        {
            if (currentTransaction.Amount <= 0)
            {
                return false;
            }
            if (currentTransaction.TransactionDate == default)
            {
                return false;
            }
            if (string.IsNullOrEmpty(currentTransaction.TransactionType))
            {
                return false;
            }
            return true;
        }    
    
    }
    public class BankParserResult
    {
        public List<TransactionInputModel> Transactions { get; set; } = new();
        public List<string> Errors { get; set; } = new();
        public bool IsValid { get; set; }
        public string FileAccountNumber { get; set; }
        public int TotalTransactionsFound => Transactions.Count;
    }
}
