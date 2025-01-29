using System;

namespace WebApp.DataAccess.Authentication;

public interface IAuthenticationRepository
{
    Task<bool> IsUserRegistered(string emailAddress);
    Task CreateUser(string emailAddress, string ciphertext);
    Task<string?> GetHashedPassword(string emailAddress);
}
