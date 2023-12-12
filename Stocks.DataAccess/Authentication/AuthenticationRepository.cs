namespace Stocks.DataAccess.Authentication;

public class AuthenticationRepository
{
    private readonly DatabaseConnection _connection;

    public AuthenticationRepository(DatabaseConnection connection) => _connection = connection;

    public void SaveUser(string emailAddress, string ciphertext)
    {

    }
}