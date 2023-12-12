namespace Stocks.DataAccess;

using Dapper;
using System.Data.Common;
using MySqlConnector;

/// <summary>
/// Abstraction so we can switch DB implementations in one place and it Just Works
/// </summary>
public class DatabaseConnection
{
    private DbConnection _connection;

    public DatabaseConnection(string connectionString)
    {
        _connection = new MySqlConnection(connectionString);
    }

    public IEnumerable<T> Query<T>(string sql)
    {
        return _connection.Query<T>(sql);
    }

    public async Task ExecuteAsync(string sql, object? parameters = null)
    {
        await _connection.ExecuteAsync(sql, parameters);
    }
}