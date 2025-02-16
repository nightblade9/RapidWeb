using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebApp.DataAccess.Authentication;
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
    private readonly IAuthenticationRepository _authRepo;
    private readonly IConfiguration _configuration;

    public LoginModel(ILogger<LoginModel> logger, IConfiguration configuration, IAuthenticationRepository authRepo, Dictionary<string, object?>? viewData)
    : base(viewData)
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
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            ViewData["Message"] = "Please fill out all fields.";
            return Page();
        }

        var passwordHash = await _authRepo.GetHashedPassword(EmailAddress);
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
        
        // TODO: put a viewbag message in
        ViewData["Message"] = "Logged in.";
        return RedirectToPage("/Index");
    }
}
