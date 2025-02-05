using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Api;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class AuthorizationCheckController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok("You're authenticated!!");
}