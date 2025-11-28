using System.ComponentModel.DataAnnotations;

namespace BanksDB.Core.Entities
{
    public class Organization
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Название организации обязательно")]
        public string Name { get; set; }
        [Required(ErrorMessage = "ИНН обязателен")]
        public string Inn { get; set; }
        public string Kpp { get; set; }
        public string Address { get; set; }
        public bool IsDeleted { get; set; }

        public ICollection<Account> Accounts { get; set; } = new List<Account>();
    }
}
