using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using WebApp.DataAccess.HealthCheck;
using WebApp.Web.Pages.Shared;

namespace WebApp.Web.Pages;

[Authorize]
public class HealthCheckModel : BasePageModel
{
    public Dictionary<string, bool> IsHealthCheckSuccessful { get; private set; } = new();
    
    private readonly ILogger<HealthCheckModel> _logger;
    private readonly IConnectionChecker _connectionChecker;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _contextAccessor;

    public HealthCheckModel(ILogger<HealthCheckModel> logger, IConnectionChecker connectionChecker, IHttpClientFactory httpClientFactory, IHttpContextAccessor contextAccessor, Dictionary<string, object?> viewData = null)
    : base(viewData)
    {
        _logger = logger;
        _connectionChecker = connectionChecker;
        _httpClientFactory = httpClientFactory;
        _contextAccessor = contextAccessor;
    }

    public override void OnPageHandlerExecuted(PageHandlerExecutedContext context)
    {
        base.OnPageHandlerExecuted(context);
        ViewData["Title"] = "Health Check";
    }

    public void OnGet()
    {
        _logger.LogInformation("Health check invoked at {UtcNow}", DateTime.UtcNow);

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
            var httpClient = _httpClientFactory.CreateClient("ApiClient");

            // Get JWT token from cookie and inject it into the correct AUTHORIZATION header
            var jwtToken = _contextAccessor.HttpContext?.Request.Cookies["ApiAuthToken"];
            if (!string.IsNullOrEmpty(jwtToken))
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtToken);
            }

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
