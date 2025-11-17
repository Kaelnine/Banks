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
        //Task<bool> LoginAsync(LoginModel model);
        //Task LogoutAsync();
        //bool IsAuthenticated { get; }
        User CurrentUser { get; }
        bool IsAuthenticated { get; }
        event Action OnAuthenticationStateChanged;
        bool Login (string username, string password);
        void Logout();
        bool HasRole(string role);
    }
    public class AuthenticationService : IAuthenticationService
    {
        //public bool IsAuthenticated { get; private set; }
        //public event Action OnAuthenticationStateChanged;
        //private readonly NavigationManager _navigationManager;
        //public UserRole CurrentUserRole { get; private set; }
        //public AuthenticationService(NavigationManager navigationManager)
        //{
        //    _navigationManager = navigationManager;
        //}

        //public async Task<bool> LoginAsync(LoginModel model)
        //{
        //    try
        //    {                
        //        await Task.Delay(500);

        //        if (!string.IsNullOrEmpty(model.Login) && !string.IsNullOrEmpty(model.Password) && model.Password.Length >= 6)
        //        {
        //            IsAuthenticated = true;
        //            OnAuthenticationStateChanged?.Invoke();
        //            return true;
        //        }

        //        return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Auth service error: {ex.Message}");
        //        return false;
        //    }
        //}
        private User _currentUser;
        public User CurrentUser => _currentUser;
        public bool IsAuthenticated => _currentUser != null;
        public event Action OnAuthenticationStateChanged;
        
        public bool Login(string username, string password)
        {
            var users = new List<User>();
            var user = users.FirstOrDefault(u => u.Username == username && u.Password == password);
            if (user != null)
            {
                _currentUser = user;
                OnAuthenticationStateChanged.Invoke();
                return true;
            }
            return false;
        }

        public void Logout()
        {
            _currentUser = null;
            OnAuthenticationStateChanged.Invoke();
        }

        public bool HasRole(string role) => _currentUser?.Role == role;

        //public async Task LogoutAsync()
        //{
        //    IsAuthenticated = false;
        //    OnAuthenticationStateChanged?.Invoke();
        //    await Task.CompletedTask;
        //}
    }
}        
    

