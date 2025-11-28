namespace BanksDB.Core.Dtos
{
    public class OrganizationDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Inn { get; set; }
        public string Kpp { get; set; }
        public string Address { get; set; }
        public bool IsDeleted { get; set; }
        public IEnumerable<AccountDto> Accounts { get; set; }
    }
}
