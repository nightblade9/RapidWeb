namespace WebApp.Web.UnitTests.Pages.Admin;

using Microsoft.AspNetCore.Http;
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
        var page = new HealthCheckModel(Substitute.For<ILogger<HealthCheckModel>>(), Substitute.For<IConnectionChecker>(), Substitute.For<IHttpClientFactory>(), Substitute.For<IHttpContextAccessor>(), viewData);

        // Act
        page.OnPageHandlerExecuted(this.CreateContext());

        // Assert
        Assert.That(viewData["Title"], Is.EqualTo("Health Check"));
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void OnGet_SetsHealthChecks(bool expected)
    {
        // Arrange
        var connectionChecker = Substitute.For<IConnectionChecker>();
        connectionChecker.CanConnectToDatabase().Returns(expected);

        var page = new HealthCheckModel(Substitute.For<ILogger<HealthCheckModel>>(), connectionChecker, Substitute.For<IHttpClientFactory>(), Substitute.For<IHttpContextAccessor>());

        // Act
        page.OnGet();

        // Assert
        Assert.That(page.IsHealthCheckSuccessful["Database Connection"], Is.EqualTo(expected));
    }
}