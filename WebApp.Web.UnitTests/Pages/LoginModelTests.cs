namespace WebApp.Web.UnitTests;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using WebApp.DataAccess.Authentication;
using WebApp.Web.Pages.Authentication;
using WebApp.Web.UnitTests.Extensions;
using WebApp.Web.UnitTests.Stubs;
using WebAppWeb.Authentication;

[TestFixture]
public class LoginModelTests
{
    private static LoginModel CreateLoginModel(ILogger<LoginModel>? logger = null, IConfiguration? configuration = null, Dictionary<string, object?>? viewData = null, IAuthenticationRepository authRepo = null)
    {
        return new LoginModel(
            logger ?? Substitute.For<ILogger<LoginModel>>(),
            configuration ?? Substitute.For<IConfiguration>(),
            authRepo ?? new AuthenticationRepositoryStub(),
            viewData);
    }

    [Test]
    public void OnPageHandlerExecuted_SetsTitleInViewData()
    {
        // Arrange
        var viewData = new Dictionary<string, object?>();
        var page = new LoginModel(Substitute.For<ILogger<LoginModel>>(), Substitute.For<IConfiguration>(), Substitute.For<IAuthenticationRepository>(), viewData);

        // Act
        page.OnPageHandlerExecuted(this.CreateContext());

        // Assert
        Assert.That(viewData["Title"], Is.EqualTo("Login"));
    }

    [Test]
    public async Task OnPostAsync_ReturnsPageResult_IfUserDoesntExist()
    {
        // Arrange
        var logger = Substitute.For<ILogger<LoginModel>>();
        var viewData = new Dictionary<string, object?>();
        var page = CreateLoginModel(logger, this.CreateConfiguration(), viewData);

        // Act
        var actual = await page.OnPostAsync();

        // Assert
        Assert.That(actual, Is.InstanceOf<PageResult>());
        Assert.That(viewData["Message"], Is.EqualTo("Authentication failed."));
    }

    // Flaky? test: if the user doesn't have a password, this runs the same code flow as the above
    // test, and it passes. Hmm. Not sure how to distinguish, since we can't spy on log messages.
    [Test]
    public async Task OnPostAsync_ReturnsPageResult_IfPasswordIsIncorrect()
    {
        // Arrange
        var logger = Substitute.For<ILogger<LoginModel>>();
        var viewData = new Dictionary<string, object?>();
        
        var authRepo = Substitute.For<IAuthenticationRepository>();
        // Real-looking but fake data, because the code tries to unpack it
        authRepo.GetHashedPassword(Arg.Any<string>()).Returns("$2a$11$aaabbbbbbbbbbccccccccccddddddddddeeeeeeeeeeffffffffff");

        var page = CreateLoginModel(logger, this.CreateConfiguration(), viewData, authRepo);
        page.Password = "any wrong password";

        // Act
        var actual = await page.OnPostAsync();

        // Assert
        Assert.That(actual, Is.InstanceOf<PageResult>());
        Assert.That(viewData["Message"], Is.EqualTo("Authentication failed."));
    }

    [Test]
    public async Task OnPostAsync_ReturnsRedirectAndSetsMessage_IfAuthenticationSucceeds()
    {
        // Arrange
        var logger = Substitute.For<ILogger<LoginModel>>();
        var viewData = new Dictionary<string, object?>();
        var plaintext = "password";
        var ciphertext = PasswordEncrypter.Hash(plaintext);
        
        var authRepo = Substitute.For<IAuthenticationRepository>();
        authRepo.GetHashedPassword(Arg.Any<string>()).Returns(ciphertext);

        var page = CreateLoginModel(logger, this.CreateConfiguration(), viewData, authRepo);
        page.Password = plaintext;
        // Act
        var actual = await page.OnPostAsync();

        // Assert
        Assert.That(actual, Is.InstanceOf<RedirectToPageResult>());
        Assert.That(viewData["Message"], Is.EqualTo("Logged in."));
    }
}