namespace WebApp.Web.UnitTests;

using Microsoft.Extensions.Configuration;
using NSubstitute;
using NUnit.Framework;
using WebApp.Web.Pages;
using WebApp.Web.UnitTests.Extensions;

[TestFixture]
public class IndexModelTests
{
    [Test]
    public void OnPageHandlerExecuted_SetsTitleInViewData()
    {
        // Arrange
        var viewData = new Dictionary<string, object?>();
        var page = new IndexModel(Substitute.For<IConfiguration>(), viewData);

        // Act
        page.OnPageHandlerExecuted(this.CreateContext());

        // Assert
        Assert.That(viewData["Title"], Is.EqualTo("Home"));
    }
}