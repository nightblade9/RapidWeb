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
    public string EmailAddress { get; set; } = default!;
    
    [BindProperty]
    [Required]
    [MinLength(12)]
    public string Password { get; set; } = default!;

    [BindProperty]
    [Display(Name = "Retype Password")]
    [Required]
    [MinLength(12)]
    [Compare(nameof(Password), ErrorMessage = "Passwords don't match.")] // doesn't work
    public string PasswordAgain { get; set; } = default!;
    
    private readonly ILogger<RegisterModel> _logger;
    private readonly IConfiguration _configuration;
    private readonly AuthenticationRepository _authRepo;

    public RegisterModel(ILogger<RegisterModel> logger, IConfiguration configuration, AuthenticationRepository authRepo)
    {
        _logger = logger;
        _configuration = configuration;
        _authRepo = authRepo;
    }

    public override void OnPageHandlerExecuted(PageHandlerExecutedContext context)
    {
        base.OnPageHandlerExecuted(context);
        ViewData["Title"] = "Register";
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

        if (Password != PasswordAgain)
        {
            ModelState.AddModelError(nameof(Password), "Passwords don't match.");
            return Page();
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var isUserRegistered = await _authRepo.IsUserRegistered(EmailAddress);
        if (isUserRegistered)
        {
            // Since we don't have forget-password functionality, this is an acceptable security trade-off;
            // namely: attackers can tell who's registered by trying to re-register.
            ModelState.AddModelError(nameof(EmailAddress), "Email address is already registered.");
            return Page();
        }


        // Save to DB here
        var passwordHash = PasswordEncrypter.Hash(Password);
        await _authRepo.CreateUser(EmailAddress, passwordHash);
        // TODO: put a viewbag message in
        return RedirectToPage("/Index");
    }
}
