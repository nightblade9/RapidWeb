using System.Diagnostics.CodeAnalysis;

namespace WebApp.DataAccess.HealthCheck;

[ExcludeFromCodeCoverage]
public class ConnectionChecker : IConnectionChecker
{
    private readonly DatabaseConnection _connection;

    public ConnectionChecker(DatabaseConnection connection) => _connection = connection;

    public async Task<bool> CanConnectToDatabase()
    {
         try
         {
            await _connection.QuerySingleAsync<int>("SELECT 1 + 2");
            return true;
        }
        catch
        {
            return false;
        }
    }
}