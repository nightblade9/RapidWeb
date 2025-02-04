using System.Data;
using Dapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;

namespace WebApp.Api.IntegrationTests;

public class InMemoryApiRunner<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing Db connection
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IDbConnection));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add an in-memory SQLite DB for testing
            services.AddTransient<IDbConnection>(_ =>
            {
                var connection = new SqliteConnection("DataSource=:memory:");
                connection.Open();
                SetupTestDatabase(connection);
                return connection;
            });
        });
    }

    private void SetupTestDatabase(IDbConnection connection)
    {
        // Create tables and seed data for your tests
        var createTableQuery = @"
            CREATE TABLE Users (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Username TEXT NOT NULL,
                Password TEXT NOT NULL
            );
            
            INSERT INTO Users (Username, Password) VALUES ('testuser', 'password123');
        ";

        connection.Execute(createTableQuery);
    }
}
