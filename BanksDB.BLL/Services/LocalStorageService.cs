using Microsoft.JSInterop;
using BanksDB.BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BanksDB.BLL.Services
{
    public class LocalStorageService : ILocalStorageService
    {
        private readonly IJSRuntime _runtime;
        public LocalStorageService(IJSRuntime runtime)
        {
            _runtime = runtime;
        }
        public async Task SetStringAsync(string key, string value)
        {
            await _runtime.InvokeVoidAsync("localStorage.setItem", key, value);
        }
        public async Task<string> GetStringAsync(string key)
        {
            return await _runtime.InvokeAsync<string>("localStorage.getItem", key);
        }
        public async Task RemoveAsync(string key)
        {
            await _runtime.InvokeVoidAsync("localStorage.removeItem", key);
        }
        public async Task<bool> ContainsKeyAsync(string key)
        {
            var value = await GetStringAsync(key);
            return value != null;
        }
    }
}
