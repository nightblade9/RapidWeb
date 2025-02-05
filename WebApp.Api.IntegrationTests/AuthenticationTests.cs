using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace WebApp.Api.IntegrationTests;

[TestFixture]
public class AuthenticationTests
{
    private WebApplicationFactory<Program> _factory;
    private HttpClient _client;

    public static string GenerateJwtToken(string emailAddress)
    {
        // TODO: this is bad, bad, bad.
        var appSettings = File.ReadAllText("../../../../WebApp.Api/appsettings.json");
        var apiConfigData = JsonConvert.DeserializeObject<JObject>(appSettings);
        var encryptionKey = apiConfigData["Jwt"]["Key"].ToString();

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(encryptionKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new Claim[]
        {   
            new Claim(ClaimTypes.Name, emailAddress),
            new Claim(ClaimTypes.Role, "User"),
        };

        var token = new JwtSecurityToken(
            issuer: apiConfigData["Jwt"]["Issuer"].ToString(),
            audience: apiConfigData["Jwt"]["Audience"].ToString(),
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    [SetUp]
    public void Setup()
    {
        // Initialize WebApplicationFactory for the API
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
    }

    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    [Test]
    public async Task AuthorizationCheckCheck_ReturnsUnauthorized_IfUserIsNotLoggedIn()
    {
        // Arrange
        var response = await _client.GetAsync("/api/authorizationcheck");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task AuthorizationCheck_ReturnsOk_IfUserIsLoggedIn()
    {
        // Arrange
        var authToken = GenerateJwtToken("test@test.com");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        var response = await _client.GetAsync("/api/authorizationcheck");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }
}
