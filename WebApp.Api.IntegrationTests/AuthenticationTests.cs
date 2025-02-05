using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;

namespace WebApp.Api.IntegrationTests;

[TestFixture]
public class AuthenticationTests
{
    private WebApplicationFactory<Program> _factory;
    private HttpClient _client;

    [SetUp]
    public void Setup()
    {
        // Initialize WebApplicationFactory for the API
        _factory = new WebApplicationFactory<Program>();
            // .WithWebHostBuilder(builder =>
            // {
            //     // Configure any test-specific settings here, e.g., in-memory DB, mock services, etc.
            // });

        _client = _factory.CreateClient();
    }

    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    [Test]
    public async Task HealthCheck_ReturnsOk()
    {
        // Arrange
        var response = await _client.GetAsync("/api/healthcheck");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }
}
