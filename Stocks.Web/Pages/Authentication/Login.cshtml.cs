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

    private readonly ILogger<IndexModel> _logger;
    private readonly AuthenticationRepository _authRepo;

    public LoginModel(ILogger<IndexModel> logger, AuthenticationRepository authRepo)
    {
        _logger = logger;
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

        var isAuthenticated = PasswordEncrypter.IsAMatch(EmailAddress, passwordHash);

        if (!isAuthenticated)
        {
            return Page();
        }

        // TODO: put a viewbag message in
        return RedirectToPage("/Index");
    }
}
