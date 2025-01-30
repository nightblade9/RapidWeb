using Microsoft.AspNetCore.Mvc;

namespace WebApp.Api.HealthCheck;

[Route("api/[controller]")]
[ApiController]
public class HealthCheckController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok("API is running!");
}