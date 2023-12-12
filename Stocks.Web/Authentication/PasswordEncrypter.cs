namespace StocksWeb.Authentication;

using BCrypt.Net;

public static class PasswordEncrypter
{
    /// <summary>
    /// Hashes a password, and returns a 60-character hash. Due to salting, calling this
    /// method multiple times will result in different hashes each time.
    /// Uses BCrypt's default generated salt scheme, which is cross-platform and stuff.
    /// </summary>
    public static string Hash(string plaintext)
    {
        return BCrypt.HashPassword(plaintext); 
    }

    /// <summary>
    /// Verifies that <c>hash</c> is the hash for <c>plaintext</c>
    /// </summary>
    public static bool IsAMatch(string plaintext, string hash)
    {
        return BCrypt.Verify(plaintext, hash);
    }
}