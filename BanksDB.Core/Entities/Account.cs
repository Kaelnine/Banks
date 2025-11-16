using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BanksDB.Core.Entities
{
    public class Account
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int OrganizationId { get; set; }
        public Organization Organization { get; set; }

        public int BankId { get; set; }
        public Bank Bank { get; set; }

        public string AccountNumber { get; set; }
        public decimal CurrentBalance { get; set; }

        public DateTime UpdateAccount { get; set; }
        public bool IsDeleted { get; set; }

        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
