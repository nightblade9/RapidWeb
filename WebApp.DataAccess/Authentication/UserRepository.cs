using System.Diagnostics.CodeAnalysis;
using WebApp.DataAccess.Migration;

namespace WebApp.DataAccess.Authentication;

[ExcludeFromCodeCoverage]
public class UserRepository : IUserRepository
{
    private readonly DatabaseConnection _connection;

    public UserRepository(DatabaseConnection connection) => _connection = connection;

    public async Task<bool> IsUserRegistered(string emailAddress)
    {
        var result = await _connection.QuerySingleOrDefaultAsync<string>($"SELECT EmailAddress FROM {TableNames.UsersTableName} WHERE EmailAddress = @email",
           new { email = emailAddress });

        return string.IsNullOrWhiteSpace(result) ? false : true;
    }

    public async Task CreateUser(string emailAddress, string ciphertext)
    {
        await _connection.ExecuteAsync($"INSERT INTO {TableNames.UsersTableName} (EmailAddress, PasswordHash) VALUES (@email, @password)",
           new { email = emailAddress, password = ciphertext });
    }

    public async Task<string?> GetHashedPassword(string emailAddress)
    {
        var result = await _connection.QuerySingleOrDefaultAsync<string>($"SELECT PasswordHash FROM {TableNames.UsersTableName} WHERE EmailAddress = @email",
           new { email = emailAddress });

        return result;
    }

    public async Task<ApplicationUser> GetUser(string emailAddress)
    {
        var result = await _connection.QuerySingleAsync<ApplicationUser>($"SELECT * FROM {TableNames.UsersTableName} WHERE EmailAddress = @email",
            new { email = emailAddress });
        
        return result;
    }
}