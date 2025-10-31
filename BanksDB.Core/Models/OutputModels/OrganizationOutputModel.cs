using BanksDB.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BanksDB.Core.Models.OutputModels
{
    public class OrganizationOutputModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Inn { get; set; }
        public string? Kpp { get; set; }
        public string? Address { get; set; }        
        public IEnumerable<AccountDto> Accounts { get; set; }
    }
}
