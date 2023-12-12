using Stocks.DataAccess.Migration;

namespace Stocks.DataAccess.Authentication;

public class AuthenticationRepository
{
    private readonly DatabaseConnection _connection;

    public AuthenticationRepository(DatabaseConnection connection) => _connection = connection;

    public async Task CreateUser(string emailAddress, string ciphertext)
    {
        await _connection.ExecuteAsync($"INSERT INTO {TableNames.UsersTableName} (EmailAddress, PasswordHash) VALUES (@email, @password)",
           new { email = emailAddress, password = ciphertext });
    }
}