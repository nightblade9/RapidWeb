using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Stocks.DataAccess;

namespace Stocks.Web.Pages;

public class HealthCheckModel : PageModel
{
    public Dictionary<string, bool> HealthCheckSuccessful = new();
    
    private readonly ILogger<IndexModel> _logger;
    private readonly ConnectionChecker _connectionChecker; 

    public HealthCheckModel(ILogger<IndexModel> logger, ConnectionChecker connectionChecker)
    {
        _logger = logger;
        _connectionChecker = connectionChecker;
    }

    public override void OnPageHandlerExecuted(PageHandlerExecutedContext context)
    {
        base.OnPageHandlerExecuted(context);
        ViewData["Title"] = "Health Check";
    }

    public async Task OnGet()
    {
       this.HealthCheckSuccessful["Database Connection"] = await _connectionChecker.CanConnectToDatabase();
    }
}
