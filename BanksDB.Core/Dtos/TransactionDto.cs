using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BanksDB.Core.Dtos
{
    public class TransactionDto
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; }
        public string Description { get; set; }
        public string CounterpartyName { get; set; }
        public string CounterpartyAccount { get; set; }
        public string CounterpartyInn { get; set; }
        public string DocumentNumber { get; set; }
        public string PayerName { get; set; }
        public string PayerInn { get; set; }
        public string PayerAccount { get; set; }
        //public decimal BalanceAfter { get; set; }
        public DateTime CreatedDate { get; set; }
        public AccountDto Account { get; set; }
        public bool IsDeleted { get; set; }
    }
}
