namespace WebApp.Web.UnitTests;

using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using WebApp.Web.Pages;
using WebApp.Web.UnitTests.Extensions;

[TestFixture]
public class ErrorModelTests
{
    [Test]
    [TestCase(" ")]
    [TestCase("abc")]
    [TestCase("666")]
    [TestCase("ab1eb03d-6aa3-4a57-8e13-5abb5c8f7763")]
    public void ShowRequestId_ReturnsTrue_IfRequestIdIsNotNullOrEmpty(string requestId)
    {
        // Arrange
        var page = new ErrorModel();
        page.RequestId = requestId;

        // Act/Assert
        Assert.That(page.ShowRequestId, Is.True);
    }

    [Test]
    public void OnPageHandlerExecuted_SetsTitleInViewData()
    {
        // Arrange
        const string expectedTitle = "Error";
        var viewData = new Dictionary<string, object?>();
        var page = new ErrorModel(viewData);
        
        // Act
        page.OnPageHandlerExecuted(this.CreateContext());
        
        // Assert
        Assert.That(viewData["Title"], Is.EqualTo(expectedTitle));
    }
}