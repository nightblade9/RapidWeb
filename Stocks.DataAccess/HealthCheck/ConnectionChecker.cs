namespace Stocks.DataAccess;
using Stocks.DataAccess;

using Dapper;
using MySqlConnector;

public class ConnectionChecker
{
    private readonly DatabaseConnection _connection;

    public ConnectionChecker(DatabaseConnection connection) => _connection = connection;

    public bool CanConnectToDatabase()
    {
         try
         {
            _connection.Query<int>("SELECT 1 + 2");
            return true;
        }
        catch (Exception e)
        {
            //throw; // uncomment to debug the error
            return false;
        }
    }
}