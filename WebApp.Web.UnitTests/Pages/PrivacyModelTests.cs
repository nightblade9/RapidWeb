namespace WebApp.Web.UnitTests.Pages;

using NUnit.Framework;
using WebApp.Web.Pages;
using WebApp.Web.UnitTests.Extensions;

[TestFixture]
public class PrivacyPolicyModelTests
{
    [Test]
    public void OnPageHandlerExecuted_SetsTitleInViewData()
    {
        // Arrange
        var viewData = new Dictionary<string, object?>();
        var page = new PrivacyModel(viewData);

        // Act
        page.OnPageHandlerExecuted(this.CreateContext());

        // Assert
        Assert.That(viewData["Title"], Is.EqualTo("Privacy Policy"));
    }
}