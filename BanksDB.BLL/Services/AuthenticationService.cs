using BanksDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BanksDB.BLL.Services.AuthenticationService;

namespace BanksDB.BLL.Services
{
    public class AuthenticationService
    {
        public interface IAuthenticationService
        {
            Task<bool> LoginAsync(LoginModel model);
            Task LogoutAsync();
            bool IsAuthenticated { get; }
        }

        public class AuthService : IAuthenticationService
        {
            public bool IsAuthenticated { get; private set; }

            public async Task<bool> LoginAsync(LoginModel model)
            {
                // Здесь должна быть реальная логика аутентификации
                // Например, вызов API или проверка в базе данных

                // Временная заглушка для демонстрации
                await Task.Delay(500); // Имитация задержки сети

                if (model.Login == "admin" && model.Password == "admin")
                {
                    IsAuthenticated = true;
                    return true;
                }

                return false;
            }

            public async Task LogoutAsync()
            {
                IsAuthenticated = false;
                await Task.CompletedTask;
            }
        }
    }
}
