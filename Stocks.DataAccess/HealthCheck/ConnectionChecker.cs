namespace Stocks.DataAccess;
using Stocks.DataAccess;

using Dapper;
using MySqlConnector;

public class ConnectionChecker
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
        catch (Exception e)
        {
            //throw; // uncomment to debug the error
            return false;
        }
    }
}