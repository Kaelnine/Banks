namespace BanksDB.Core.Models
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public bool IsDeleted { get; set; }
        public string RoleDisplayName => Role switch
        {
            "Accountant" => "Бухгалтер",
            "Director" => "Директор",
            _ => Role
        };
    }
}
