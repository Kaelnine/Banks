using BanksDB.Core.Entities;

namespace BanksDB.Core.Models.OutputModels
{
    public class DailySummaryOutputModel
    {
        public DateTime Date { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public int TransactionCount { get; set; }
        public decimal TotalAmount => Transactions.Sum(t => t.Amount);
        public List<Transaction> Transactions { get; set; } = new();
    }
}
