using System.ComponentModel.DataAnnotations;

namespace BanksDB.Core.Entities
{
    public class Account
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Название счета обязательно")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Организация обязательна")]
        [Range(1, int.MaxValue, ErrorMessage = "Выберите организацию")]
        public int OrganizationId { get; set; }        
        public Organization Organization { get; set; }
        [Required(ErrorMessage = "Банк обязателен")]
        [Range(1, int.MaxValue, ErrorMessage = "Выберите банк")]
        public int BankId { get; set; }        
        public Bank Bank { get; set; }
        [Required(ErrorMessage = "Номер счета обязателен")]
        public string AccountNumber { get; set; }
        public decimal InitialBalance { get; set; }
        public decimal CurrentBalance { get; set; }

        public DateTime UpdateAccount { get; set; }
        public bool IsDeleted { get; set; }

        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
