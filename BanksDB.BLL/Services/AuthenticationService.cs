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
        bool HasAccessToEdit();
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
            _users = users;
            _localStorage = localStorageService;
            _mapper = mapper;
        }
        
        public async Task InitializeAsync()
        {
            try
            {
                Console.WriteLine("Начало инициализации");
                await TryRestoreUserFromStorageAsync();
                Console.WriteLine($"Инициализация завершена. Аутентификация: {IsAuthenticated}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка инициализации: {ex}");
            }
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
                _currentUser = _mapper.Map<User>(user);
                if (user != null)
                {
                    await _localStorage.SetStringAsync("auth_username", user.UserName);
                    await _localStorage.SetStringAsync("auth_role", user.Role);
                    await _localStorage.SetStringAsync("auth_fullName", user.FullName ?? "");
                    OnAuthenticationStateChanged?.Invoke();
                    return true;
                }
                
                if (_authenticationStateProvider is CustomAuthStateProvider customAuthStateProvider)
                {
                    await customAuthStateProvider.LoginAsync(user.UserName, user.Role, user.FullName);
                }
                
                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка входа: {ex}");
                return false;
            }
            
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
            
        }

        public bool HasAccessToEdit()
        {
            return IsAuthenticated && (CurrentUser?.Role == "Accountat");
        }
        
    }
}        
    

