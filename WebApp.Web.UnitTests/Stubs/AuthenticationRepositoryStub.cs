using WebApp.DataAccess.Authentication;

namespace WebApp.Web.UnitTests.Stubs;

public class AuthenticationRepositoryStub : IAuthenticationRepository
{
    public async Task CreateUser(string emailAddress, string ciphertext)
    {
    }

    public async Task<string?> GetHashedPassword(string emailAddress)
    {
        return null;
    }

    public async Task<bool> IsUserRegistered(string emailAddress)
    {
        return false;
    }
}
