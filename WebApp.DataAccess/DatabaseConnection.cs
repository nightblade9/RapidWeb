namespace WebApp.DataAccess;

using Dapper;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using System.Diagnostics.CodeAnalysis;


/// <summary>
/// Abstraction so we can switch DB implementations in one place and it Just Works
/// </summary>
[ExcludeFromCodeCoverage]
public class DatabaseConnection
{
    private readonly DbConnection _connection;

    public DatabaseConnection(string connectionString)
    {
        // To use MySQL, substitute with an instance of MySqlConnection
        _connection = new SqliteConnection(connectionString);
    }

    public async Task<T> QuerySingleAsync<T>(string sql, object? parameters = null)
    {
        return await _connection.QuerySingleAsync<T>(sql, parameters);
    }

    public async Task<T?> QuerySingleOrDefaultAsync<T>(string sql, object? parameters = null)
    {
        return await _connection.QuerySingleOrDefaultAsync<T>(sql, parameters);
    }

    public async Task ExecuteAsync(string sql, object? parameters = null)
    {
        await _connection.ExecuteAsync(sql, parameters);
    }
}