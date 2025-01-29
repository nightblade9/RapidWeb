namespace WebApp.Web.UnitTests;

using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NUnit.Framework;
using WebApp.Web.Pages;

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
        page.OnPageHandlerExecuted(CreateContext());
        
        // Assert
        Assert.That(viewData["Title"], Is.EqualTo(expectedTitle));
    }

    [Test]
    public void OnGet_SetsRequestIdToHttpContextTraceIdentifier_IfActivityIdIsNull()
    {
        // Arrange
        var page = new ErrorModel();
        var expected = page.HttpContext.TraceIdentifier;

        // Act
        page.OnGet();

        // Assert
        Assert.That(page.RequestId, Is.EqualTo(expected));
    }

    private PageHandlerExecutedContext CreateContext()
    {
        var httpContext = new DefaultHttpContext() { TraceIdentifier = Guid.NewGuid().ToString() };

        var actionContext = new Microsoft.AspNetCore.Mvc.ActionContext(
            httpContext,
            new Microsoft.AspNetCore.Routing.RouteData(),
            new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()); 

        var pageContext = new PageContext(actionContext);

        var toReturn = new PageHandlerExecutedContext(
            pageContext,
            new List<IFilterMetadata>(),
            new Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure.HandlerMethodDescriptor(),
            new object()
        );

        return toReturn;
    }
}