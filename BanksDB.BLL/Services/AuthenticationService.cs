using BanksDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BanksDB.BLL.Services
{
    public interface IAuthenticationService
    {
        Task<bool> LoginAsync(LoginModel model);
        Task LogoutAsync();
        bool IsAuthenticated { get; }
    }
    public class AuthenticationService : IAuthenticationService
    {
        public bool IsAuthenticated { get; private set; }
        public event Action OnAuthenticationStateChanged;

        public async Task<bool> LoginAsync(LoginModel model)
        {
            try
            {
                // Имитация задержки сети
                await Task.Delay(500);

                // Простая проверка для демонстрации
                if (!string.IsNullOrEmpty(model.Login) && !string.IsNullOrEmpty(model.Password) && model.Password.Length >= 6)
                {
                    IsAuthenticated = true;
                    OnAuthenticationStateChanged?.Invoke();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Auth service error: {ex.Message}");
                return false;
            }
        }

        public async Task LogoutAsync()
        {
            IsAuthenticated = false;
            OnAuthenticationStateChanged?.Invoke();
            await Task.CompletedTask;
        }
    }
}

        //public class AuthService : IAuthenticationService
        //{
        //    public bool IsAuthenticated { get; private set; }

        //    public async Task<bool> LoginAsync(LoginModel model)
        //    {
        //        // Здесь должна быть реальная логика аутентификации
        //        // Например, вызов API или проверка в базе данных

        //        // Временная заглушка для демонстрации
        //        await Task.Delay(500); // Имитация задержки сети

        //        if (model.Login == "admin" && model.Password == "admin")
        //        {
        //            IsAuthenticated = true;
        //            return true;
        //        }

        //        return false;
        //    }

        //    public async Task LogoutAsync()
        //    {
        //        IsAuthenticated = false;
        //        await Task.CompletedTask;
        //    }
        //}
    

