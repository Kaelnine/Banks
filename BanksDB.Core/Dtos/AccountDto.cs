using System.Collections.ObjectModel;

namespace BanksDB.Core.Dtos
{
    public class AccountDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public int BankId { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public decimal CurrentBalance { get; set; }
        //
        //public string AccountType { get; set; }
        //
        public DateTime UpdateAccount { get; set; }
        public OrganizationDto Organization { get; set; }
        public BankDto Bank { get; set; }
        public bool IsDeleted { get; set; }
        public ObservableCollection<TransactionDto> Transactions { get; set; } = new ObservableCollection<TransactionDto>();

    }
}
