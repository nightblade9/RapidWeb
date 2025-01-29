using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Web.Pages;

[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public class ErrorModel : PageModel
{
    public string? RequestId { get; set; }
    
    // The page's ViewData. Injectable for unit testing, of course.
    private IDictionary<string, object?> _viewData; 

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    public ErrorModel(IDictionary<string, object?>? viewDataDictionary = null)
    {
        _viewData = viewDataDictionary ?? ViewData;
    }

    public override void OnPageHandlerExecuted(PageHandlerExecutedContext context)
    {
        base.OnPageHandlerExecuted(context);

        _viewData["Title"] = "Error";
    }

    public void OnGet()
    {
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
    }
}

