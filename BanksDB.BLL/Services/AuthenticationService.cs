using AutoMapper;
using BanksDB.BLL.Interfaces;
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
        User CurrentUser { get; }
        bool IsAuthenticated { get; }
        event Action OnAuthenticationStateChanged;        
        Task<bool> LoginAsync(string username, string password, bool rememberMe = false);        
        Task LogoutAsync();
        Task InitializeAsync();
    }
    public class AuthenticationService : IAuthenticationService
    {        
        private readonly IUserRepository _users;
        private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly IMapper _mapper;        
        private User _currentUser;
        public User CurrentUser => _currentUser;
        public bool IsAuthenticated => _currentUser != null;
        public event Action OnAuthenticationStateChanged;

        public AuthenticationService(IUserRepository users, ILocalStorageService localStorageService, IMapper mapper)
        {
            //_context = context;
            //_authenticationStateProvider = authenticationStateProvider;
            _users = users;
            _localStorage = localStorageService;
            _mapper = mapper;
        }

        //public bool IsAuthenticated => _context.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
        public async Task InitializeAsync()
        {
            await TryRestoreUserFromStorageAsync();
        }

        private async Task TryRestoreUserFromStorageAsync()
        {
            try
            {
                var savedUsername = await _localStorage.GetStringAsync("auth_username");
                if (string.IsNullOrEmpty(savedUsername))
                {
                    Console.WriteLine("Нет сохраненных польхователей");
                    return;
                }

                var savedRole = await _localStorage.GetStringAsync("auth_role");
                var savedFullName = await _localStorage.GetStringAsync("auth_fullName");

                _currentUser = new User
                {
                    UserName = savedUsername,
                    Role = savedRole ?? "User",
                    FullName = savedFullName ?? ""
                };

                Console.WriteLine($"Пользователь восстановлен: {_currentUser.UserName} ({_currentUser.Role})");
                OnAuthenticationStateChanged?.Invoke();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка восстановления пользователя: {ex.Message}");
            }
        }

        public async Task<bool> LoginAsync(string username, string password, bool rememberMe = false)
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
                if (user != null)
                {
                    await _localStorage.SetStringAsync("auth_username", user.UserName);
                    await _localStorage.SetStringAsync("auth_role", user.Role);
                    await _localStorage.SetStringAsync("auth_fullName", user.FullName ?? "");
                    OnAuthenticationStateChanged?.Invoke();
                    return true;
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
                //OnAuthenticationStateChanged?.Invoke();
                //Console.WriteLine($"Пользователь {username}: успешный вход. Роль: {user.Role}");
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
            try
            {                
                _currentUser = null;                
                await _localStorage.RemoveAsync("auth_username");
                await _localStorage.RemoveAsync("auth_role");
                await _localStorage.RemoveAsync("auth_fullName");                                
                OnAuthenticationStateChanged?.Invoke();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при выходе: {ex}");
            }
        
            //await _context.HttpContext.SignOutAsync("Cookies");

            //////////////это работало
            //if (_authenticationStateProvider is CustomAuthStateProvider customProvider)
            //{
            //    customProvider.Logout();
            //}

            //_currentUser = null;
            //OnAuthenticationStateChanged?.Invoke();
            //Console.WriteLine("Успешный выход");
            ////////////////////////вот досюда
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
    

