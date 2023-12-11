using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Stocks.Web.Pages.Authentication;

public class RegisterModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public RegisterModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public override void OnPageHandlerExecuted(PageHandlerExecutedContext context)
    {
        base.OnPageHandlerExecuted(context);
        ViewData["Title"] = "Register";
    }

    public void OnGet()
    {

    }
}
