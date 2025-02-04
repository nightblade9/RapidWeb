using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using WebApp.DataAccess.Authentication;
using WebApp.Shared.Authentication;
using WebApp.Web.Pages.Shared;

namespace WebApp.Web.Pages.Authentication;

public class LoginModel : BasePageModel
{
    [BindProperty]
    [Required]
    [EmailAddress]
    public string EmailAddress { get; set; } = default!;

    [BindProperty]
    [Required]
    public string Password { get; set; } = default!;

    private readonly ILogger<LoginModel> _logger; // security logger
    private readonly IUserRepository _userRepo;
    
    // Can only be null in unit tests, to bypass untestable code!
    private readonly IHttpContextAccessor? _httpContext;
    private readonly IConfiguration _configuration;

    public LoginModel(ILogger<LoginModel> logger, IConfiguration configuration, IUserRepository userRepo, IHttpContextAccessor httpContext, Dictionary<string, object?>? viewData = null)
    : base(viewData)
    {
        _logger = logger;
        _userRepo = userRepo;
        _configuration = configuration;
        _httpContext = httpContext;
    }

    public override void OnPageHandlerExecuted(PageHandlerExecutedContext context)
    {
        base.OnPageHandlerExecuted(context);
        ViewData["Title"] = "Login";
    }

    public IActionResult OnGet()
    {
        if (!_configuration.GetValue<bool>("FeatureToggles:AllowUserRegistration"))
        {
            return NotFound();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!_configuration.GetValue<bool>("FeatureToggles:AllowUserRegistration"))
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            ViewData["Message"] = "Please fill out all fields.";
            return Page();
        }

        var passwordHash = await _userRepo.GetHashedPassword(EmailAddress);
        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            _logger.LogInformation("Login attempt for non-existing user {EmailAddress} at {UtcNow}", EmailAddress, DateTime.UtcNow);
            ViewData["Message"] = "Authentication failed.";
            return Page(); // User doesn't exist in DB
        }

        var isAuthenticated = PasswordEncrypter.IsAMatch(Password, passwordHash);

        if (!isAuthenticated)
        {
            _logger.LogInformation("{EmailAddress} failed to authenticate at {UtcNow}", EmailAddress, DateTime.UtcNow);
            ViewData["Message"] = "Authentication failed.";
            return Page();
        }

        _logger.LogInformation("{EmailAddress} logged in at {UtcNow}", EmailAddress, DateTime.UtcNow);

        await SetUserCookieAndToken();
        
        ViewData["Message"] = "Logged in.";
        return RedirectToPage("/Index");
    }

    private async Task SetUserCookieAndToken()
    {
        // Fetch the user's role from the database
        var user = await _userRepo.GetUser(EmailAddress);

        // Create claims for the user
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, EmailAddress),
            new Claim(ClaimTypes.Role, user.Role ?? "User") // Default role if none assigned
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        // Set the cookie in the HTTP Context. This is what "logs in" the user.
        if (_httpContext != null)
        {
            await _httpContext.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal,
                new AuthenticationProperties() {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(30),
            });
                
            // Generate a JWT token for API access
            var token = GenerateJwtToken(user, claims);

            _httpContext.HttpContext.Response.Cookies.Append("ApiAuthToken", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None, // Prevents CSRF
                Expires = DateTime.UtcNow.AddMinutes(30)
            });
        }
    }

    private string GenerateJwtToken(ApplicationUser user, List<Claim> claims)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
