using BanksDB.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace BanksDB.Core.Models.OutputModels
{
    public class DailySummaryOutputModel
    {
        public DateTime Date { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public int TransactionCount { get; set; }//=> Transactions.Count;
        public decimal TotalAmount => Transactions.Sum(t => t.Amount);
        public List<Transaction> Transactions { get; set; } = new();
    }
}
