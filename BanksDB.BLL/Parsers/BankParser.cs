using BanksDB.Core.Enums;
using BanksDB.Core.Models.InputModels;
using System.Globalization;
using System.Text;

namespace BanksDB.BLL.Parsers
{
    public class BankParser
    {
        static BankParser()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }
        public BankParserResult ParseFile(Stream fileStream, string expectedAccountNumber = null)
        {
            var transactions = new List<TransactionInputModel>();
            var errors = new List<string>();
            bool accountMismatch = false;
            using var reader = new StreamReader(fileStream, Encoding.GetEncoding(1251));
            string line;
            var inDocumentSection = false;
            TransactionInputModel currentTransaction = null;
            var lineNumber = 0;
            string fileAccountNumber = null;
            var supportedDocTypes = new List<string>
            {
                "Платежное поручение",
                "Банковский ордер",
                "Платежное требование",
                "Прочее"
            };
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
                    //if (line.StartsWith("СекцияДокумент=Платежное поручение"))
                    //{
                    //    inDocumentSection = true;
                    //    currentTransaction = new TransactionInputModel();
                    //    continue;
                    //}
                    if (line.StartsWith("СекцияДокумент="))
                    {
                        var docType = line.Replace("СекцияДокумент=", "").Trim();
                        if (supportedDocTypes.Contains(docType))
                        {
                            inDocumentSection = true;
                            currentTransaction = new TransactionInputModel();
                            continue;
                        }
                        else
                        {                            
                            inDocumentSection = false;
                            currentTransaction = null;
                            continue;
                        }
                    }
                    if (line == "КонецДокумента" && inDocumentSection)
                    {
                        inDocumentSection = false;

                        if (currentTransaction != null)
                        {
                            DetermineTransactionTypeAndCounterparty(currentTransaction);
                            if (IsValidTransaction(currentTransaction))
                            {
                                transactions.Add(currentTransaction);
                            }
                            else
                            {
                                errors.Add($"Строка {lineNumber}: Невалидная транзакция - {currentTransaction.DocumentNumber}");
                            }
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
                accountMismatch = true;
            }
            return new BankParserResult
            {
                Transactions = transactions,
                Errors = errors,
                IsValid = !errors.Any() && !accountMismatch,
                FileAccountNumber = fileAccountNumber,
                HasAccountMismatch = accountMismatch
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
                    if (DateTime.TryParseExact(value, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))//dd-MM-yyyy
                    {
                        currentTransaction.TransactionDate = date;
                    }
                    else
                    {
                        DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
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
                //currentTransaction.CounterpartyName = value;
                //-----
                case "Получатель1":
                    if (string.IsNullOrEmpty(currentTransaction.CounterpartyName))
                    {
                        currentTransaction.CounterpartyName = value;
                    }
                //-----
                    break;
                case "ПолучательРасчСчет":
                //if (!string.IsNullOrEmpty(value))
                //{
                //    currentTransaction.CounterpartyAccount = value;
                //}
                //-----
                case "ПолучательСчет":
                    if (!string.IsNullOrEmpty(value) && string.IsNullOrEmpty(currentTransaction.CounterpartyAccount))
                    {
                        currentTransaction.CounterpartyAccount = value;
                    }
                //-----
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
                //currentTransaction.PayerName = value;
                //-----
                case "Плательщик1":
                    if (string.IsNullOrEmpty(currentTransaction.PayerName))
                    {
                        currentTransaction.PayerName = value;
                    }
                //-----
                    break;
                case "ПлательщикИНН":
                    currentTransaction.PayerInn = value;
                    break;
                case "ПлательщикРасчСчет":
                //currentTransaction.PayerAccount = value;
                //-----
                case "ПлательщикСчет":
                    if (!string.IsNullOrEmpty(value))
                    {
                        currentTransaction.PayerAccount = value;
                    }
                //-----
                    break;
                case "ДатаСписано":
                    //if (DateTime.TryParseExact(value, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var writeOffDate))
                    //{
                    //    currentTransaction.WriteOffDate = writeOffDate;
                    //}
                    //-----
                    if (!string.IsNullOrEmpty(value))
                    {
                        if (DateTime.TryParseExact(value, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var writeOffDate))
                        {
                            currentTransaction.WriteOffDate = writeOffDate;
                        }
                        else if (DateTime.TryParseExact(value, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out writeOffDate))
                        {
                            currentTransaction.WriteOffDate = writeOffDate;
                        }
                        else if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out writeOffDate))
                        {
                            currentTransaction.WriteOffDate = writeOffDate;
                        }
                    }
                    //-----
                    break;
                case "ДатаПоступило":
                    //if (DateTime.TryParseExact(value, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var receiptDate))
                    //{
                    //    currentTransaction.ReceiptDate = receiptDate;
                    //}
                    //-----
                    if (!string.IsNullOrEmpty(value))
                    {
                        if (DateTime.TryParseExact(value, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var receiptDate))
                        {
                            currentTransaction.ReceiptDate = receiptDate;
                            // Для приходных операций используем дату поступления как дату транзакции
                            currentTransaction.TransactionDate = receiptDate;
                        }
                        else if (DateTime.TryParseExact(value, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out receiptDate))
                        {
                            currentTransaction.ReceiptDate = receiptDate;
                            currentTransaction.TransactionDate = receiptDate;
                        }
                        else if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out receiptDate))
                        {
                            currentTransaction.ReceiptDate = receiptDate;
                            currentTransaction.TransactionDate = receiptDate;
                        }
                    }
                    //-----
                    break;
                    //-----
                case "ВидОплаты":
                    currentTransaction.PaymentType = value;
                    break;
                    //-----
            }
        }
        private void DetermineTransactionTypeAndCounterparty(TransactionInputModel currentTransaction)
        {
            //if (currentTransaction.ReceiptDate != default && currentTransaction.WriteOffDate == default)
            if (currentTransaction.ReceiptDate != default)
            {
                currentTransaction.TransactionType = TransactionType.Приход.ToString();
                currentTransaction.DisplayCounterparty = currentTransaction.PayerName;
                currentTransaction.DisplayCounterpartyInn = currentTransaction.PayerInn;
                currentTransaction.DisplayCounterpartyAccount = currentTransaction.PayerAccount;
                currentTransaction.TransactionDate = currentTransaction.ReceiptDate;
            }
            //else if (currentTransaction.WriteOffDate != default && currentTransaction.ReceiptDate == default)
            else if (currentTransaction.WriteOffDate != default)
            {
                currentTransaction.TransactionType = TransactionType.Расход.ToString();
                currentTransaction.DisplayCounterparty = currentTransaction.CounterpartyName;
                currentTransaction.DisplayCounterpartyInn = currentTransaction.CounterpartyInn;
                currentTransaction.DisplayCounterpartyAccount = currentTransaction.CounterpartyAccount;
                currentTransaction.TransactionDate = currentTransaction.WriteOffDate;
            }
            else
            {
                if (string.IsNullOrEmpty(currentTransaction.TransactionType))
                {
                    currentTransaction.TransactionType = TransactionType.Приход.ToString();
                    currentTransaction.DisplayCounterparty = currentTransaction.PayerName;
                    currentTransaction.DisplayCounterpartyInn = currentTransaction.PayerInn;
                    currentTransaction.DisplayCounterpartyAccount = currentTransaction.PayerAccount;
                }
                // Если нет ни той, ни другой даты - определяем по счетам
                //if (!string.IsNullOrEmpty(currentTransaction.PayerAccount) &&
                //    currentTransaction.PayerAccount == currentTransaction.FileAccountNumber)
                //{
                //    currentTransaction.TransactionType = TransactionType.Расход.ToString();
                //    currentTransaction.DisplayCounterparty = currentTransaction.CounterpartyName;
                //    currentTransaction.DisplayCounterpartyInn = currentTransaction.CounterpartyInn;
                //    currentTransaction.DisplayCounterpartyAccount = currentTransaction.CounterpartyAccount;
                //}
                //else
                //{
                //    currentTransaction.TransactionType = TransactionType.Приход.ToString();
                //    currentTransaction.DisplayCounterparty = currentTransaction.PayerName;
                //    currentTransaction.DisplayCounterpartyInn = currentTransaction.PayerInn;
                //    currentTransaction.DisplayCounterpartyAccount = currentTransaction.PayerAccount;
                //}
            }
            if (string.IsNullOrEmpty(currentTransaction.DisplayCounterparty))
            {
                currentTransaction.DisplayCounterparty = currentTransaction.CounterpartyName ??
                                                       currentTransaction.PayerName ??
                                                       "Не указан";
            }

            if (string.IsNullOrEmpty(currentTransaction.DisplayCounterpartyInn))
            {
                currentTransaction.DisplayCounterpartyInn = currentTransaction.CounterpartyInn ??
                                                           currentTransaction.PayerInn ??
                                                           string.Empty;
            }

            if (string.IsNullOrEmpty(currentTransaction.DisplayCounterpartyAccount))
            {
                currentTransaction.DisplayCounterpartyAccount = currentTransaction.CounterpartyAccount ??
                                                               currentTransaction.PayerAccount ??
                                                               string.Empty;
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
        public bool HasAccountMismatch { get; set; } = false;
    }
}
