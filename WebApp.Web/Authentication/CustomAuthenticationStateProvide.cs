using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace WebApp.Web.Authentication;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CustomAuthenticationStateProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var user = _httpContextAccessor.HttpContext.User;
        var identity = user?.Identity ?? new ClaimsIdentity();
        var principal = new ClaimsPrincipal(identity);

        return new AuthenticationState(principal);
    }

    // Call this method to notify authentication state changes (e.g., when the user logs out)
    public void MarkUserAsAuthenticated(ClaimsPrincipal user)
    {
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    public void MarkUserAsLoggedOut()
    {
        var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymous)));
    }
}
