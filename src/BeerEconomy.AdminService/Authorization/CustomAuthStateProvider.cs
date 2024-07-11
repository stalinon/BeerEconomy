using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace BeerEconomy.AdminService.Authorization;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private ClaimsPrincipal _user = new(new ClaimsIdentity());
    
    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return Task.FromResult(new AuthenticationState(_user));
    }

    public async Task<bool> Login(string username, string password)
    {
        var adminUsername = Environment.GetEnvironmentVariable("ADMIN_USERNAME");
        var adminPassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD");

        if (username == adminUsername && password == adminPassword)
        {
            var claims = new[] {
                new Claim(ClaimTypes.Name, username)
            };
            var identity = new ClaimsIdentity(claims, "BasicAuthentication");
            _user = new ClaimsPrincipal(identity);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_user)));
            return true;
        }

        return false;
    }

    public void Logout()
    {
        var identity = new ClaimsIdentity();
        var user = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }
}
