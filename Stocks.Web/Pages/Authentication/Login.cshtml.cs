using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Stocks.DataAccess.Authentication;
using StocksWeb.Authentication;

namespace Stocks.Web.Pages.Authentication;

public class LoginModel : PageModel
{
    [BindProperty]
    [Required]
    [EmailAddress]
    public string EmailAddress { get; set; } = default!;

    [BindProperty]
    [Required]
    public string Password { get; set; } = default!;

    private readonly ILogger<IndexModel> _securityLogger;
    private readonly AuthenticationRepository _authRepo;

    public LoginModel(ILogger<IndexModel> securityLogger, AuthenticationRepository authRepo)
    {
        _securityLogger = securityLogger;
        _authRepo = authRepo;
    }

    public override void OnPageHandlerExecuted(PageHandlerExecutedContext context)
    {
        base.OnPageHandlerExecuted(context);
        ViewData["Title"] = "Login";
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
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
            _securityLogger.LogInformation($"{EmailAddress} failed to authenticate at {DateTime.UtcNow}");
            return Page();
        }

        _securityLogger.LogInformation($"{EmailAddress} logged in at {DateTime.UtcNow}");
        // TODO: put a viewbag message in
        return RedirectToPage("/Index");
    }
}
