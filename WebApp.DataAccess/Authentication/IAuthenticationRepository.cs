namespace WebApp.DataAccess.Authentication;

public interface IAuthenticationRepository
{
    Task<bool> IsUserRegistered(string emailAddress);
    Task CreateUser(string emailAddress, string ciphertext);

    /// <summary>
    /// Returns the hashed password associated with this email address.
    /// If there isn't one (user doesn't exist), returns null.
    /// </summary>
    /// <param name="emailAddress"></param>
    /// <returns></returns>
    Task<string?> GetHashedPassword(string emailAddress);
}
