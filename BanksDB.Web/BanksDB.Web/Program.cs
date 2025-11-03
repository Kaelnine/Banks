using AutoMapper;
using BanksDB.BLL.Interfaces;
using BanksDB.BLL.Mapping;
using BanksDB.BLL.Services;
using BanksDB.Core.Interfaces;
using BanksDB.DAL.Data;
using BanksDB.Web.Components;
using DBBanks.DAL.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using static System.Net.Mime.MediaTypeNames;


namespace BanksDB.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddRazorPages(options =>
            {
                options.RootDirectory = "/Components/Pages";
            });
            builder.Services.AddServerSideBlazor();
            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
            builder.Services.AddAntiforgery();
            builder.Services.AddScoped<IBankRepository, BankRepository>();
            builder.Services.AddScoped<IAccountRepository, AccountRepository>();
            builder.Services.AddScoped<IOrganizationRepository, OrganizationRepository>();
            builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddDbContext<BankDbContext>(options =>
                 options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });
            var serviceProvider = builder.Services.BuildServiceProvider();
            try
            {
                var mapper = serviceProvider.GetRequiredService<IMapper>();
                mapper.ConfigurationProvider.AssertConfigurationIsValid();
            }
            catch (AutoMapperConfigurationException ex)
            {                
                Console.WriteLine($"Ошибка конфигурации маппинга: {ex.Message}");
            }
            
            // блок авторизации
            //builder.Services.AddRazorPages();
            //builder.Services.AddServerSideBlazor();
            //builder.Services.AddHttpContextAccessor();

            //builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            //    .AddCookie(options =>
            //{
            //    options.Cookie.Name = "auth_token";
            //    options.LoginPath = "/login";
            //    options.LogoutPath = "/logout";
            //    options.AccessDeniedPath = "/access-denied";
            //    options.ExpireTimeSpan = TimeSpan.FromDays(7);
            //});

            //builder.Services.AddAuthorizationCore();

            //builder.Services.AddAntiforgery();

            //builder.Services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();
            //builder.Services.AddScoped<AuthenticationStateProvider, CookieAuthenticationStateProvider>();
            // конец блока авторизации

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents()
                .AddInteractiveWebAssemblyComponents();
            

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            

            app.MapStaticAssets();
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode()
                .AddInteractiveWebAssemblyRenderMode()
                .AddAdditionalAssemblies(typeof(Client._Imports).Assembly);

            // тоже связано с авторизацией
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseAntiforgery();
            app.MapRazorPages();
            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");
            //

            app.Run();
        }
    }
}
