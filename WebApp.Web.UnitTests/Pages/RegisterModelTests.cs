namespace WebApp.Web.UnitTests;

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
        var page = new RegisterModel(Substitute.For<ILogger<RegisterModel>>(), Substitute.For<IConfiguration>(), new AuthenticationRepositoryStub(), viewData);

        // Act
        page.OnPageHandlerExecuted(this.CreateContext());

        // Assert
        Assert.That(viewData["Title"], Is.EqualTo("Register"));
    }
}