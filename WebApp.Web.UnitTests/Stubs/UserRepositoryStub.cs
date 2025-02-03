using WebApp.DataAccess.Authentication;

namespace WebApp.Web.UnitTests.Stubs;

public class UserRepositoryStub : IUserRepository
{
    public async Task CreateUser(string emailAddress, string ciphertext)
    {
    }

    public async Task<string?> GetHashedPassword(string emailAddress)
    {
        return null;
    }

    public Task<ApplicationUser> GetUser(string emailAddress)
    {
        return Task.FromResult(new ApplicationUser() { UserName = emailAddress });
    }

    public async Task<bool> IsUserRegistered(string emailAddress)
    {
        return false;
    }
}
