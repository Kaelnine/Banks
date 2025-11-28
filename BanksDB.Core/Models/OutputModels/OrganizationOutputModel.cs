using BanksDB.Core.Dtos;

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
