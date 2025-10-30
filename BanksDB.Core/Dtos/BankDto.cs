using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BanksDB.Core.Dtos
{
    public class BankDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Bik { get; set; }
        public string CorrespondentAccount { get; set; }
        public string Address { get; set; }
        public bool IsDeleted { get; set; }
    }
}
