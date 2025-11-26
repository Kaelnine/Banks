using AutoMapper;
using BanksDB.BLL.Interfaces;
using BanksDB.BLL.Mapping;
using BanksDB.BLL.Parsers;
using BanksDB.BLL.Services;
using BanksDB.DAL.Data;
using BanksDB.Core.Interfaces;
using BanksDB.DAL.Repositories;
using DBBanks.DAL.Repositories;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;


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
            builder.Services.AddTransient<IAccountService, AccountService>();
            builder.Services.AddTransient<ITransactionService, TransactionService>();
            builder.Services.AddServerSideBlazor();
            builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();
            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
            builder.Services.AddScoped<AuthenticationService>();            
            builder.Services.AddAntiforgery();
            builder.Services.AddScoped<IBankService, BankService>();
            builder.Services.AddScoped<IBankRepository, BankRepository>();
            builder.Services.AddScoped<IAccountRepository, AccountRepository>();
            builder.Services.AddScoped<IOrganizationService, OrganizationService>();
            builder.Services.AddScoped<IOrganizationRepository, OrganizationRepository>();
            builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
            builder.Services.AddScoped<BankParser>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
            builder.Services.AddHttpContextAccessor();            
            builder.Services.AddDbContextFactory<BankDbContext>(options =>
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

            
            builder.Services.AddAuthentication("Cookies").AddCookie("Cookies", options =>
                {
                    options.LoginPath = "/login";
                });
            builder.Services.AddAuthorization(options =>
            {
                // Бухгалтер
                options.AddPolicy("AccountantOnly", policy =>
                    policy.RequireRole("Accountant"));

                // Директор
                options.AddPolicy("DirectorOnly", policy =>
                    policy.RequireRole("Director"));
            });


            builder.Services.AddAuthorizationCore();
            

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();
            
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);


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
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseAntiforgery();
            app.MapRazorPages();
            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");
            

            app.Run();
        }
    }
}
