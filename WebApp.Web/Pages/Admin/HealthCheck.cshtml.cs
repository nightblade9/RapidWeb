using Microsoft.AspNetCore.Mvc.Filters;
using WebApp.DataAccess.HealthCheck;
using WebApp.Web.Pages.Shared;

namespace WebApp.Web.Pages;

public class HealthCheckModel : BasePageModel
{
    public Dictionary<string, bool> IsHealthCheckSuccessful { get; private set; } = new();
    
    private readonly ILogger<HealthCheckModel> _logger;
    private readonly IConnectionChecker _connectionChecker;
    private readonly IHttpClientFactory _httpClientFactory;

    public HealthCheckModel(ILogger<HealthCheckModel> logger, IConnectionChecker connectionChecker, IHttpClientFactory httpClientFactory, Dictionary<string, object?> viewData = null)
    : base(viewData)
    {
        _logger = logger;
        _connectionChecker = connectionChecker;
        _httpClientFactory = httpClientFactory;
    }

    public override void OnPageHandlerExecuted(PageHandlerExecutedContext context)
    {
        base.OnPageHandlerExecuted(context);
        ViewData["Title"] = "Health Check";
    }

    public void OnGet()
    {
        _logger.LogInformation("Health check invoked at {UtcNow}", DateTime.UtcNow);

        // TODO: use Task.WaitAll instead
        Task<bool>[] allTasks = [_connectionChecker.CanConnectToDatabase(), IsApiConnectionSuccessful()];
        Task.WaitAll(allTasks);

        IsHealthCheckSuccessful["Database Connection"] = allTasks[0].Result;
        IsHealthCheckSuccessful["API Connection"] = allTasks[1].Result;
    }

    private async Task<bool> IsApiConnectionSuccessful()
    {
        bool isApiConnectionSuccessful;
        try
        {
            var httpClient = _httpClientFactory.CreateClient();
            var apiResponse = await httpClient.GetAsync("https://localhost:7145/api/healthcheck");
            isApiConnectionSuccessful = apiResponse.IsSuccessStatusCode;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error calling health-check API");
            isApiConnectionSuccessful = false;
        }

        return isApiConnectionSuccessful;
    }
}
