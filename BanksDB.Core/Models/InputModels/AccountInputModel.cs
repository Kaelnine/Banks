using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BanksDB.Core.Models.InputModels
{
    public class AccountInputModel
    {
        [Required(ErrorMessage = "Название счета обязательно")]
        public string Name { get; set; }

        [Required(ErrorMessage = "ID организации обязателен")]
        public int OrganizationId { get; set; }

        [Required(ErrorMessage = "ID банка обязателен")]
        public int BankId { get; set; }

        [Required(ErrorMessage = "Номер счета обязателен")]
        [StringLength(20, ErrorMessage = "Номер счета должен содержать 20 цифр")]
        public string AccountNumber { get; set; }

        public decimal CurrentBalance { get; set; }

        [Required(ErrorMessage = "Тип счета обязателен")]
        public int AccountType { get; set; }
    }
}

