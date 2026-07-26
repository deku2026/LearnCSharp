using System.Security.Claims;

namespace LearnBlazor.Blazor_Part13_AuthSecurityBoundary.Auth;

public sealed record User(string Name, string[] Roles)
{
    public ClaimsPrincipal ToPrincipal()
    {
        ClaimsIdentity identity = new(
            claims: Roles.Select(r => new Claim(ClaimTypes.Role, r))
                .Append(new Claim(ClaimTypes.Name, Name)),
            authenticationType: "simple");
        return new ClaimsPrincipal(identity);
    }
}
