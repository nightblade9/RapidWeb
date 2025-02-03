using System.Diagnostics.CodeAnalysis;
using FluentMigrator.Runner;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using WebApp.DataAccess;
using WebApp.DataAccess.Authentication;
using WebApp.DataAccess.HealthCheck;
using WebApp.DataAccess.Migration.Migrations;
using WebApp.Web.Authentication;

namespace WebApp.Web;

[ExcludeFromCodeCoverage]
public class Program
{
    public async static Task Main(string[] args)
    {
        /// Uses an implicit partial class. Manually exclude from code coverage on the tool side.
        /// In SonarQube, add it to general file exclusions (**/Program.cs)

        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        // Routing
        {
            builder.Services.AddRazorPages(options => {
                options.Conventions.AddPageRoute("/Authentication/Login", "login");
                options.Conventions.AddPageRoute("/Authentication/Register", "register");
            });
        }

        // Add CORS stuff
        {
            var services = builder.Services;
            services.AddCors();
        }

        // Dependency injection
        builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
        builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
        builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>(); // Must be singleton

        // DB config. TODO: move to API
        var connectionString = builder.Configuration.GetConnectionString("Default");
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Please define a connection string in your config.");
            }

            // Dependency injection for our app
            builder.Services.AddTransient(x => new DatabaseConnection(connectionString));
            builder.Services.AddTransient<IConnectionChecker, ConnectionChecker>();
            builder.Services.AddTransient<IUserRepository, UserRepository>();

            // DB Migrations runner config
            builder.Services.AddFluentMigratorCore().ConfigureRunner(r => r
                // .AddMySql5()
                .AddSQLite()
                // Migrations connection string
                .WithGlobalConnectionString(connectionString)
                // Assembly we check for migrations 
                .ScanIn(typeof(AddUsersTable_001).Assembly).For.Migrations())
                .AddLogging(l => l.AddFluentMigratorConsole());
        };

        builder.Services.AddHttpClient();
        builder.Services.AddEndpointsApiExplorer();

        // Configure authentication
        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Login"; // Redirect to login page
                options.AccessDeniedPath = "/Index"; // Redirect if forbidden

                options.Cookie.Name = "WebApp.Identity"; // Name the cookie
                options.Cookie.SameSite = SameSiteMode.Lax; // Or SameSiteMode.None if you need cross-origin
                options.Cookie.HttpOnly = true; // Security measure, no JS access
                options.Cookie.IsEssential = true; // Required for Blazor Server, must be true for session to persist
                
                 // Allow cookies over HTTP in development
                options.Cookie.SecurePolicy = builder.Environment.IsDevelopment() 
                    ? CookieSecurePolicy.None 
                    : CookieSecurePolicy.SameAsRequest;
            });

        builder.Services.AddAuthorization();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();
        app.UseAuthorization();
        app.MapRazorPages();

        app.MapControllers();

        // Enable authentication and authz
        app.UseAuthentication();
        app.UseAuthorization(); // Required for [Authorize] to work

        // Run migrations
        using (var migratorScope = app.Services.CreateScope())
        {
            var services = migratorScope.ServiceProvider;
            var migrationRunner = services.GetService<IMigrationRunner>();

            if (migrationRunner == null)
            {
                return;
            }

            migrationRunner.ValidateVersionOrder();
            migrationRunner.MigrateUp();
        }

        await app.RunAsync();
    }
}