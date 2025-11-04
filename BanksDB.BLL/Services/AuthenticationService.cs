using BanksDB.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.VisualStudio.Services.UserAccountMapping;


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
        private readonly NavigationManager _navigationManager;
        public UserRole CurrentUserRole { get; private set; }
        public AuthenticationService(NavigationManager navigationManager)
        {
            _navigationManager = navigationManager;
        }

        public async Task<bool> LoginAsync(LoginModel model)
        {
            try
            {                
                await Task.Delay(500);
                
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
    

