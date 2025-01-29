using Microsoft.AspNetCore.Mvc.Filters;
using WebApp.DataAccess.HealthCheck;
using WebApp.Web.Pages.Shared;

namespace WebApp.Web.Pages;

public class HealthCheckModel : BasePageModel
{
    public Dictionary<string, bool> IsHealthCheckSuccessful { get; private set; } = new();
    
    private readonly ILogger<HealthCheckModel> _logger;
    private readonly IConnectionChecker _connectionChecker;

    public HealthCheckModel(ILogger<HealthCheckModel> logger, IConnectionChecker connectionChecker, Dictionary<string, object?> viewData = null)
    : base(viewData)
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
