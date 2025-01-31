using NUnit.Framework;
using WebApp.Api.HealthCheck;

namespace WebApp.Api.UnitTests.HealthCheck;

[TestFixture]
public class HealthCheckControllerTests
{
    [Test]
    public void Get_ReturnsOkResult()
    {
        // Arrange
        var controller = new HealthCheckController();

        // Act
        var actual = controller.Get();

        // Assert
        Assert.That(actual, Is.InstanceOf<Microsoft.AspNetCore.Mvc.OkObjectResult>());
    }
}
