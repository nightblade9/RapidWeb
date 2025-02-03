using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebApp.DataAccess.Authentication;
using WebApp.Web.Authentication;
using WebApp.Web.Pages.Shared;
using WebAppWeb.Authentication;

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
    private readonly IHttpContextAccessor _httpContext;
    private readonly IConfiguration _configuration;
    private readonly CustomAuthenticationStateProvider _authStateProvider;

    public LoginModel(ILogger<LoginModel> logger, IConfiguration configuration, IUserRepository userRepo, IHttpContextAccessor httpContext, AuthenticationStateProvider authStateProvider, Dictionary<string, object?>? viewData = null)
    : base(viewData)
    {
        _logger = logger;
        _userRepo = userRepo;
        _configuration = configuration;
        _httpContext = httpContext;
        _authStateProvider = authStateProvider as CustomAuthenticationStateProvider;
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

        await SetUserCookie();
        
        ViewData["Message"] = "Logged in.";
        return RedirectToPage("/Index");
    }

    private async Task SetUserCookie()
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

        // Sign in the user
        _authStateProvider.MarkUserAsAuthenticated(claimsPrincipal);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal,
            new AuthenticationProperties() {
                IsPersistent = true,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(30),
        });
    }
}
