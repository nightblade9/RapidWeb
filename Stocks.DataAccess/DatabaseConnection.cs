namespace Stocks.DataAccess;

using Dapper;
using System.Data.Common;
using MySqlConnector;
using Microsoft.Data.Sqlite;

/// <summary>
/// Abstraction so we can switch DB implementations in one place and it Just Works
/// </summary>
public class DatabaseConnection
{
    private DbConnection _connection;

    public DatabaseConnection(string connectionString)
    {
        // _connection = new MySqlConnection(connectionString);
        _connection = new SqliteConnection(connectionString);
    }

    public async Task<T> QuerySingleAsync<T>(string sql, object? parameters = null)
    {
        return await _connection.QuerySingleAsync<T>(sql, parameters);
    }

    public async Task<T> QuerySingleOrDefaultAsync<T>(string sql, object? parameters = null)
    {
        return await _connection.QuerySingleOrDefaultAsync<T>(sql, parameters);
    }

    public async Task ExecuteAsync(string sql, object? parameters = null)
    {
        await _connection.ExecuteAsync(sql, parameters);
    }
}