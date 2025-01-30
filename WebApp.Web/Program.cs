using System.Diagnostics.CodeAnalysis;
using FluentMigrator.Runner;
using WebApp.DataAccess;
using WebApp.DataAccess.Authentication;
using WebApp.DataAccess.HealthCheck;
using WebApp.DataAccess.Migration.Migrations;

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

        // add services to DI container
        {
            var services = builder.Services;
            services.AddCors();
        }

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
            builder.Services.AddTransient<IAuthenticationRepository, AuthenticationRepository>();

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
        builder.Services.AddSwaggerGen();

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