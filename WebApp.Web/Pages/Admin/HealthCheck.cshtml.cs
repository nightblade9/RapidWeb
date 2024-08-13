using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.DataAccess;

namespace WebApp.Web.Pages;

public class HealthCheckModel : PageModel
{
    internal Dictionary<string, bool> IsHealthCheckSuccessful = new();
    
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
        _logger.LogInformation("Health check invoked at {UtcNow}", DateTime.UtcNow);
        IsHealthCheckSuccessful["Database Connection"] = await _connectionChecker.CanConnectToDatabase();
    }
}
