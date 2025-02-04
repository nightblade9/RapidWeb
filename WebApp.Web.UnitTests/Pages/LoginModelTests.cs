namespace WebApp.Web.UnitTests.Pages.Authentication;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using WebApp.DataAccess.Authentication;
using WebApp.Shared.Authentication;
using WebApp.Web.Pages.Authentication;
using WebApp.Web.UnitTests.Extensions;
using WebApp.Web.UnitTests.Stubs;

[TestFixture]
public class LoginModelTests
{
    private static LoginModel CreateLoginModel(ILogger<LoginModel>? logger = null, IConfiguration? configuration = null, IHttpContextAccessor? contextAccessor = null, Dictionary<string, object?>? viewData = null, IUserRepository userRepo = null)
    {
        return new LoginModel(
            logger ?? Substitute.For<ILogger<LoginModel>>(),
            configuration ?? Substitute.For<IConfiguration>(),
            userRepo ?? new UserRepositoryStub(),
            contextAccessor,
            viewData);
    }

    [Test]
    public void OnGet_ReturnsNotFoundResult_IfFeatureToggleIsDisabled()
    {
        // Arrange
        var page = CreateLoginModel(configuration: this.CreateConfiguration(false), contextAccessor: Substitute.For<IHttpContextAccessor>());

        // Act
        var actual = page.OnGet();

        // Assert
        Assert.That(actual, Is.InstanceOf<NotFoundResult>());
    }
    
    [Test]
    public void OnGet_ReturnsPageResult_IfFeatureToggleIsEnabled()
    {
        // Arrange
        var page = CreateLoginModel(configuration: this.CreateConfiguration());

        // Act
        var actual = page.OnGet();

        // Assert
        Assert.That(actual, Is.InstanceOf<PageResult>());
    }
    
    [Test]
    public void OnPageHandlerExecuted_SetsTitleInViewData()
    {
        // Arrange
        var viewData = new Dictionary<string, object?>();
        var page = new LoginModel(Substitute.For<ILogger<LoginModel>>(), Substitute.For<IConfiguration>(), Substitute.For<IUserRepository>(), Substitute.For<IHttpContextAccessor>(), viewData);

        // Act
        page.OnPageHandlerExecuted(this.CreateContext());

        // Assert
        Assert.That(viewData["Title"], Is.EqualTo("Login"));
    }

    [Test]
    public async Task OnPostAsync_ReturnsNotFoundResult_IfFeatureToggleIsDisabled()
    {
        // Arrange
        var page = CreateLoginModel(configuration: this.CreateConfiguration(false), contextAccessor: Substitute.For<IHttpContextAccessor>());

        // Act
        var actual = await page.OnPostAsync();

        // Assert
        Assert.That(actual, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public async Task OnPostAsync_ReturnsPageResult_IfUserDoesntExist()
    {
        // Arrange
        var logger = Substitute.For<ILogger<LoginModel>>();
        var viewData = new Dictionary<string, object?>();
        var page = CreateLoginModel(logger, this.CreateConfiguration(true), null, viewData);

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
        
        var userRepo = Substitute.For<IUserRepository>();
        // Real-looking but fake data, because the code tries to unpack it
        userRepo.GetHashedPassword(Arg.Any<string>()).Returns("$2a$11$aaabbbbbbbbbbccccccccccddddddddddeeeeeeeeeeffffffffff");

        var page = CreateLoginModel(logger, this.CreateConfiguration(true), null, viewData, userRepo);
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
        
        var userRepo = Substitute.For<IUserRepository>();
        userRepo.GetHashedPassword(Arg.Any<string>()).Returns(ciphertext);
        var loggedInUser = new ApplicationUser()
        {
            Id = 999,
            PasswordHash = "blah",
            Role = "grandmaster",
            UserName = "gm@chess.comz",
        };

        userRepo.GetUser(Arg.Any<string>()).Returns(Task.FromResult(loggedInUser));

        // Null IHttpContextChecker bypasses a context.HttpContext.SignInAsync call that we can't mock and that requires extensive DI to configure it correctly
        var page = CreateLoginModel(logger, this.CreateConfiguration(true), contextAccessor: null, viewData, userRepo);
        page.EmailAddress = loggedInUser.UserName;
        page.Password = plaintext;

        // Act
        var actual = await page.OnPostAsync();

        // Assert
        Assert.That(actual, Is.InstanceOf<RedirectToPageResult>());
        Assert.That(viewData["Message"], Is.EqualTo("Logged in."));
    }
}