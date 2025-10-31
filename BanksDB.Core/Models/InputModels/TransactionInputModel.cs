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
        [Required(ErrorMessage = "Сумма обязательна")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Сумма должна быть больше 0")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Тип транзакции обязателен")]
        public string TransactionType { get; set; }

        public string? Description { get; set; }
        public string? CounterpartyName { get; set; }
        public string? CounterpartyAccount { get; set; }
        public string? CounterpartyInn { get; set; }
        public string? DocumentNumber { get; set; }
    }
}
