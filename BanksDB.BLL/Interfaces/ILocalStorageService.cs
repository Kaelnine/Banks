namespace BanksDB.BLL.Interfaces
{
    public interface ILocalStorageService
    {
        Task SetStringAsync(string key, string value);
        Task<string> GetStringAsync(string key);
        Task RemoveAsync(string key);
        Task<bool> ContainsKeyAsync(string key);
    }
}
