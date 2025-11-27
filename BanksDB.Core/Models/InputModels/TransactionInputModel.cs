using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BanksDB.Core.Models.InputModels
{
    public class TransactionInputModel
    {      
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        public string TransactionType { get; set; }

        public string? Description { get; set; }
        public string? CounterpartyName { get; set; }
        public string? CounterpartyAccount { get; set; }
        public string? CounterpartyInn { get; set; }
        public string? DocumentNumber { get; set; }
        [Required]
        public DateTime TransactionDate { get; set; }
        [Required]
        public int AccountId { get; set; }
        public string PayerName { get; set; }
        public string PayerInn { get; set; }
        public string PayerAccount { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime WriteOffDate { get; set; }
        public DateTime ReceiptDate { get; set; }
    }
}
