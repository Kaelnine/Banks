namespace BanksDB.Core.Models.OutputModels
{
    public class AccountOutputModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string OrganizationName { get; set; }
        public string BankName { get; set; } = string.Empty;
        public string AccountNumber { get; set; }
        public decimal CurrentBalance { get; set; }
        public string BankBik { get; set; }
        //public string AccountTypeName { get; set; }
    }
}
