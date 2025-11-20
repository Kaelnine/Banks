using AutoMapper;
using BanksDB.BLL.Security;
using BanksDB.Core.Interfaces;
using BanksDB.Core.Models;
using BanksDB.DAL.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.Services.UserAccountMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


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
        //bool Login (string username, string password);
        Task<bool> LoginAsync(string username, string password);
        //void Logout();
        Task LogoutAsync();
        //bool HasRole(string role);
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
        //private User _currentUser;
        //public User CurrentUser => _currentUser;
        //public bool IsAuthenticated => _currentUser != null;
        //public event Action OnAuthenticationStateChanged;
        //private readonly IHttpContextAccessor _context;
        private readonly IUserRepository _users;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly IMapper _mapper;
        //public bool IsAuthenticated { get; private set; }
        //public event Action<bool> OnAuthenticationStateChanged;
        private User _currentUser;
        public User CurrentUser => _currentUser;
        public bool IsAuthenticated => _currentUser != null;
        public event Action OnAuthenticationStateChanged;

        public AuthenticationService(IUserRepository users, AuthenticationStateProvider authenticationStateProvider, IMapper mapper)
        {
            //_context = context;
            _authenticationStateProvider = authenticationStateProvider;
            _users = users;
            _mapper = mapper;
        }

        //public bool IsAuthenticated => _context.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

        public async Task<bool> LoginAsync(string username, string password)
        {
            try
            {
                var user = await _users.GetByNameAsync(username);
                if (user == null)
                {
                    Console.WriteLine($"Пользователь {username} не найден");
                    return false;
                }
                if (!PasswordHasher.VerifyPassword(password, user.PasswordHash))
                {
                    Console.WriteLine($"Неверный пароль пользователя {username}");
                    return false;
                }
                //_currentUser = _mapper.Map<User>(user);
                //var claims = new List<Claim>
                //{
                //    new Claim(ClaimTypes.Name, user.UserName),
                //    new Claim(ClaimTypes.Role, user.Role),
                //    new Claim("FullName", user.FullName ?? ""),
                //    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                //};
                //var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                //var principal = new ClaimsPrincipal(identity);
                if (_authenticationStateProvider is CustomAuthStateProvider customAuthStateProvider)
                {
                    await customAuthStateProvider.LoginAsync(user.UserName, user.Role, user.FullName);
                }
                OnAuthenticationStateChanged?.Invoke();
                Console.WriteLine($"Пользователь {username}: успешный вход. Роль: {user.Role}");
                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка входа: {ex}");
                return false;
            }
            //var user = await _users.GetByNameAsync(username);
            //if (user == null) return false;
            //if (!PasswordHasher.VerifyPassword(password, user.PasswordHash)) return false;
            //var claims = new List<Claim>
            //{
            //    new Claim(ClaimTypes.Name, user.UserName),
            //    new Claim(ClaimTypes.Role, user.Role),
            //    new Claim("FullName", user.FullName ?? "")
            //};
            //var identity = new ClaimsIdentity(claims, "Cookies");
            //var principal = new ClaimsPrincipal(identity);
            //await _context.HttpContext.SignInAsync("Cookies",  principal);
            //return true;
        }

        public async Task LogoutAsync()
        {
            //await _context.HttpContext.SignOutAsync("Cookies");
            if (_authenticationStateProvider is CustomAuthStateProvider customProvider)
            {
                customProvider.Logout();
            }

            _currentUser = null;
            OnAuthenticationStateChanged?.Invoke();
            Console.WriteLine("Успешный выход");
        }
        //public bool Login(string username, string password)
        //{
        //    var users = new List<User>();
            
        //    var user = users.FirstOrDefault(u => u.UserName == username && u.Password == password);
        //    if (user != null)
        //    {
        //        _currentUser = user;
        //        OnAuthenticationStateChanged.Invoke();
        //        return true;
        //    }
        //    return false;
        //}

        //public void Logout()
        //{
        //    _currentUser = null;
        //    OnAuthenticationStateChanged.Invoke();
        //}

        //public bool HasRole(string role) => _currentUser?.Role == role;

        //public async Task LogoutAsync()
        //{
        //    IsAuthenticated = false;
        //    OnAuthenticationStateChanged?.Invoke();
        //    await Task.CompletedTask;
        //}
    }
}        
    

