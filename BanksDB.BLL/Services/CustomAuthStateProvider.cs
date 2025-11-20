using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


namespace BanksDB.BLL.Services
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private ClaimsPrincipal _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            return Task.FromResult(new AuthenticationState(_currentUser));
        }
        public Task<bool> LoginAsync(string username, string role, string fullName)
        {
            try
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, role),
                    new Claim("FullName", fullName),
                    new Claim(ClaimTypes.NameIdentifier, username)
                };
                var identity = new ClaimsIdentity(claims, "CustomAuth");
                _currentUser = new ClaimsPrincipal(identity);
                NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
                return Task.FromResult(true);
            }
            catch
            {
                return Task.FromResult(false);
            }
        }
        public void Logout()
        {
            _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }

}
