using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Web.UnitTests.PageExtensions;

public static class PageModelExtensions
{
    
    public static PageHandlerExecutedContext CreateContext(this PageModel p)
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
