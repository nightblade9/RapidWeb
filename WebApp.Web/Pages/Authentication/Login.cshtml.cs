using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.DataAccess.Authentication;
using WebAppWeb.Authentication;

namespace WebApp.Web.Pages.Authentication;

public class LoginModel : PageModel
{
    [BindProperty]
    [Required]
    [EmailAddress]
    public string EmailAddress { get; set; } = default!;

    [BindProperty]
    [Required]
    public string Password { get; set; } = default!;

    private readonly ILogger<LoginModel> _logger; // security logger
    private readonly AuthenticationRepository _authRepo;
    private readonly IConfiguration _configuration;

    public LoginModel(ILogger<LoginModel> logger, IConfiguration configuration, AuthenticationRepository authRepo)
    {
        _logger = logger;
        _authRepo = authRepo;
        _configuration = configuration;
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
            return Page();
        }

        var passwordHash = await _authRepo.GetHashedPassword(EmailAddress);
        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            return Page(); // User doesn't exist in DB
        }

        var isAuthenticated = PasswordEncrypter.IsAMatch(Password, passwordHash);

        if (!isAuthenticated)
        {
            _logger.LogInformation("{EmailAddress} failed to authenticate at {UtcNow}", EmailAddress, DateTime.UtcNow);
            return Page();
        }

        _logger.LogInformation("{EmailAddress} logged in at {UtcNow}", EmailAddress, DateTime.UtcNow);
        
        // TODO: put a viewbag message in
        return RedirectToPage("/Index");
    }
}
