using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Stocks.DataAccess.Authentication;
using StocksWeb.Authentication;

namespace Stocks.Web.Pages.Authentication;

public class RegisterModel : PageModel
{
    private const int MinimumPasswordLength = 12;

    [BindProperty]
    [Required]
    [EmailAddress]
    [Display(Name = "Email Address")]
    public string EmailAddress { get; set; }
    
    [BindProperty]
    [Required]
    [MinLength(12)]
    public string Password { get; set; }

    [BindProperty]
    [Display(Name = "Retype Password")]
    [Required]
    [MinLength(12)]
    [Compare(nameof(Password), ErrorMessage = "Passwords don't match.")] // doesn't work
    public string PasswordAgain { get; set; }
    
    private readonly ILogger<IndexModel> _logger;
    private readonly AuthenticationRepository _authRepo;

    public RegisterModel(ILogger<IndexModel> logger, AuthenticationRepository authRepo)
    {
        _logger = logger;
        _authRepo = authRepo;
    }

    public override void OnPageHandlerExecuted(PageHandlerExecutedContext context)
    {
        base.OnPageHandlerExecuted(context);
        ViewData["Title"] = "Register";
    }

    public void OnGet()
    {

    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (Password != PasswordAgain)
        {
            ModelState.AddModelError(nameof(Password), "Passwords don't match.");
        }

        await Task.CompletedTask;

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var passwordHash = PasswordEncrypter.Hash(Password);
        await _authRepo.CreateUser(EmailAddress, passwordHash);
        // TODO: put a viewbag message in
        return RedirectToPage("/Index");
    }
}
