namespace WebApp.Web.UnitTests;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using WebApp.Web.Pages.Authentication;
using WebApp.Web.UnitTests.Extensions;
using WebApp.Web.UnitTests.Stubs;

[TestFixture]
public class RegisterModelTests
{
    [Test]
    public void OnPageHandlerExecuted_SetsTitleInViewData()
    {
        // Arrange
        var viewData = new Dictionary<string, object?>();
        var page = CreateRegisterModel(viewData: viewData);

        // Act
        page.OnPageHandlerExecuted(this.CreateContext());

        // Assert
        Assert.That(viewData["Title"], Is.EqualTo("Register"));
    }

    [Test]
    public void OnGet_ReturnsNotFoundResult_IfFeatureToggleIsDisabled()
    {
        // Arrange
        var config = CreateConfiguration(new()
        {
            {"FeatureToggles:AllowUserRegistration", "false" }
        });

        var page = CreateRegisterModel(config);

        // Act
        var actual = page.OnGet();

        // Assert
        Assert.That(actual, Is.InstanceOf<NotFoundResult>());
    }

    public RegisterModel CreateRegisterModel(IConfiguration configuration = null, Dictionary<string, object?>? viewData = null)
    {
        return new RegisterModel(Substitute.For<ILogger<RegisterModel>>(), configuration ?? Substitute.For<IConfiguration>(), new AuthenticationRepositoryStub(), viewData);
    }

    public IConfiguration CreateConfiguration(Dictionary<string, string> configValues = null)
    {
        var configuration = new ConfigurationBuilder()
        .AddInMemoryCollection(configValues ?? new())
        .Build();

        return configuration;
    }
}