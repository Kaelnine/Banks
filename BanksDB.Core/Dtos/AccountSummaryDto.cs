using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BanksDB.Core.Dtos
{
    public class AccountSummaryDto
    {
        public int Id { get; set; }
        public string OrganizationName { get; set; }
        public string OrganizationInn { get; set; }
        public string BankName { get; set; }
        public string BankBik { get; set; }
        public string AccountNumber { get; set; }
        public decimal CurrentBalance { get; set; }
        //public string AccountType { get; set; }
    }
}
