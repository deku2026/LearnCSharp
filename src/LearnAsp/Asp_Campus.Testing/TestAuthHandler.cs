using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Campus.Testing;

public sealed class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string SchemeName = "Test";

    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("X-Test-User", out StringValues userHeader) ||
            string.IsNullOrWhiteSpace(userHeader))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        string sub = userHeader.ToString();
        string role = Request.Headers.TryGetValue("X-Test-Role", out StringValues roleHeader)
            ? roleHeader.ToString()
            : "Student";
        string collegeId = Request.Headers.TryGetValue("X-Test-College", out StringValues collegeHeader)
            ? collegeHeader.ToString()
            : "college-1";
        string scopes = Request.Headers.TryGetValue("X-Test-Scope", out StringValues scopeHeader)
            ? scopeHeader.ToString()
            : "campus.read campus.write";

        List<Claim> claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, sub),
            new("sub", sub),
            new(ClaimTypes.Role, role),
            new("role", role),
            new("college_id", collegeId),
            new("scope", scopes),
        };

        ClaimsIdentity identity = new ClaimsIdentity(claims, SchemeName);
        ClaimsPrincipal principal = new ClaimsPrincipal(identity);
        AuthenticationTicket ticket = new AuthenticationTicket(principal, SchemeName);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
