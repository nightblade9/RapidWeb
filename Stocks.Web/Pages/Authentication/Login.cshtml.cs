using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Stocks.Web.Pages.Authentication;

public class LoginModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public LoginModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {

    }
}
