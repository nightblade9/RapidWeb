namespace WebApp.Web.UnitTests;

using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using WebApp.DataAccess.HealthCheck;
using WebApp.Web.Pages;
using WebApp.Web.UnitTests.Extensions;

[TestFixture]
public class HealthCheckModelTests
{
    [Test]
    public void OnPageHandlerExecuted_SetsTitleInViewData()
    {
        // Arrange
        var viewData = new Dictionary<string, object?>();
        var page = new HealthCheckModel(Substitute.For<ILogger<HealthCheckModel>>(), Substitute.For<IConnectionChecker>(), viewData);

        // Act
        page.OnPageHandlerExecuted(this.CreateContext());

        // Assert
        Assert.That(viewData["Title"], Is.EqualTo("Health Check"));
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public async Task OnGet_SetsHealthChecks(bool expected)
    {
        // Arrange
        var connectionChecker = Substitute.For<IConnectionChecker>();
        connectionChecker.CanConnectToDatabase().Returns(expected);

        var page = new HealthCheckModel(Substitute.For<ILogger<HealthCheckModel>>(), connectionChecker);

        // Act
        await page.OnGet();

        // Assert
        Assert.That(page.IsHealthCheckSuccessful["Database Connection"], Is.EqualTo(expected));
    }
}