using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BanksDB.Core.Entities
{
    public class Bank
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Название банка обязательно")]
        public string Name { get; set; }
        [Required(ErrorMessage = "БИК обязателен")]
        public string Bik { get; set; }
        public string CorrespondentAccount { get; set; }
        public string Address { get; set; }
        public bool IsDeleted { get; set; }

        public ICollection<Account> Accounts { get; set; } = new List<Account>();
    }
}
