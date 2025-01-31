namespace WebApp.Web.UnitTests.Pages.Authentication;

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

[TestFixture]
public class RegisterModelTests
{
    private static RegisterModel CreateRegisterModel(IConfiguration? configuration = null, Dictionary<string, object?>? viewData = null, IAuthenticationRepository authRepo = null)
    {
        return new RegisterModel(
            Substitute.For<ILogger<RegisterModel>>(),
            configuration ?? Substitute.For<IConfiguration>(),
            authRepo ?? new AuthenticationRepositoryStub(),
            viewData);
    }

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
        var page = CreateRegisterModel(this.CreateConfiguration(false));

        // Act
        var actual = page.OnGet();

        // Assert
        Assert.That(actual, Is.InstanceOf<NotFoundResult>());
    }
    
    [Test]
    public void OnGet_ReturnsPageResult_IfFeatureToggleIsEnabled()
    {
        // Arrange
        var page = CreateRegisterModel(this.CreateConfiguration());

        // Act
        var actual = page.OnGet();

        // Assert
        Assert.That(actual, Is.InstanceOf<PageResult>());
    }

    [Test]
    public async Task OnPostAsync_ReturnsNotFoundResult_IfFeatureToggleIsDisabled()
    {
        // Arrange
        var page = CreateRegisterModel(this.CreateConfiguration(false));

        // Act
        var actual = await page.OnPostAsync();

        // Assert
        Assert.That(actual, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public async Task OnPostAsync_ReturnsPageWithModelError_IfPasswordsDontMatch()
    {
        // Arrange
        var page = CreateRegisterModel(this.CreateConfiguration(true));
        page.Password = "right";
        page.PasswordAgain = "r1gh7";

        // Act
        var actual = await page.OnPostAsync();

        // Assert
        Assert.That(actual, Is.InstanceOf<PageResult>());
        var modelState = page.ModelState;
        Assert.That(modelState.ErrorCount, Is.EqualTo(1));
        Assert.That(modelState.Any(a => a.Key == nameof(page.Password)));
    }

    [Test]
    public async Task OnPostAsync_ReturnsPageWithModelError_IfUserIsAlreadyRegistered()
    {
        // Arrange
        var authRepo = Substitute.For<IAuthenticationRepository>();
        authRepo.IsUserRegistered(Arg.Any<string>()).Returns(true);
        var page = CreateRegisterModel(this.CreateConfiguration(true), authRepo: authRepo);

        // Act
        var actual = await page.OnPostAsync();

        // Assert
        Assert.That(actual, Is.InstanceOf<PageResult>());
        var modelState = page.ModelState;
        Assert.That(modelState.ErrorCount, Is.EqualTo(1));
        Assert.That(modelState.Any(a => a.Key == nameof(page.EmailAddress)));
    }

    [Test]
    public async Task OnPostAsync_CreatesUserAndRedirectsToHomePage_IfUserRegistersSuccessfully()
    {
        // Arrange
        var authRepo = Substitute.For<IAuthenticationRepository>();
        authRepo.IsUserRegistered(Arg.Any<string>()).Returns(false);
        
        var page = CreateRegisterModel(this.CreateConfiguration(true), authRepo: authRepo);
        page.EmailAddress = "test@test.com";
        // Password is too short, but enforcement is configured via attributes, so we can't test/enforce it.
        page.Password = "password";
        page.PasswordAgain = "password";

        // Act
        var actual = await page.OnPostAsync();

        // Assert
        Assert.That(actual, Is.InstanceOf<RedirectToPageResult>());
        var modelState = page.ModelState;
        Assert.That(modelState.ErrorCount, Is.EqualTo(0));
        await authRepo.Received(1).CreateUser(Arg.Any<string>(), Arg.Any<string>());
    }
}