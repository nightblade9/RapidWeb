using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Api.HealthCheck;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class HealthCheckController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok("API is running!");
}