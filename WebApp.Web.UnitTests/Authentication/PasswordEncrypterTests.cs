using System;
using NUnit.Framework;
using WebAppWeb.Authentication;

namespace WebApp.Web.UnitTests.Authentication;

[TestFixture]
public class PasswordEncrypterTests
{
    [Test]
    public void Hash_ReturnsASaltedBcryptHash()
    {
        // Arrange
        var plaintext = "Some sort of crazy pass-phrase.";

        // Act
        var hash1 = PasswordEncrypter.Hash(plaintext);
        var hash2 = PasswordEncrypter.Hash(plaintext);

        // Assert
        Assert.That(hash1.Length, Is.EqualTo(60));
        Assert.That(hash1.StartsWith("$2a$11$")); // BCrypt hash with salt

        // Salt is included, so hashes should be different
        Assert.That(hash1, Is.Not.EqualTo(hash2));
    }
}
