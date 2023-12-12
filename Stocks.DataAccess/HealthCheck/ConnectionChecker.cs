namespace StocksSpectator.DataAccess;

using Dapper;
using MySqlConnector;

public class ConnectionChecker
{
    private readonly MySqlConnection _connection;

    public ConnectionChecker(MySqlConnection connection) => _connection = connection;

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