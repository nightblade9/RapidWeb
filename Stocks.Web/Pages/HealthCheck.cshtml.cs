using Dapper;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlConnector;

namespace Stocks.Web.Pages;

public class HealthCheckModel : PageModel
{
    public Dictionary<string, bool> HealthCheckSuccessful = new();
    private readonly ILogger<IndexModel> _logger;
    private readonly MySqlConnection _connection;

    public HealthCheckModel(
        ILogger<IndexModel> logger,
        MySqlConnection connection)
    {
        _logger = logger;
        _connection = connection;
    }

    public override void OnPageHandlerExecuted(PageHandlerExecutedContext context)
    {
        base.OnPageHandlerExecuted(context);
        ViewData["Title"] = "Health Check";
    }

    public void OnGet()
    {
        try {
            var result = _connection.Query<int>("SELECT 1 + 2");
            HealthCheckSuccessful["Database Connection"] = true;
        }
        catch (Exception e)
        {
            //throw; // uncomment to debug the error
            HealthCheckSuccessful["Database Connection"] = false;
        }
    }
}
