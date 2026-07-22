using System.Globalization;
using System.Security.Claims;
using System.Text.Json;

namespace Campus.Security;

public static class BackchannelLogoutTokenValidator
{
    private const string LogoutEvent =
        "http://schemas.openid.net/event/backchannel-logout";

    public static bool IsValid(ClaimsPrincipal principal)
    {
        if (principal.HasClaim(claim => claim.Type == "nonce") ||
            string.IsNullOrWhiteSpace(principal.FindFirstValue("jti")) ||
            !long.TryParse(
                principal.FindFirstValue("iat"),
                NumberStyles.None,
                CultureInfo.InvariantCulture,
                out _))
        {
            return false;
        }

        string? events = principal.FindFirstValue("events");
        if (string.IsNullOrWhiteSpace(events))
        {
            return false;
        }

        try
        {
            using JsonDocument document = JsonDocument.Parse(events);
            return document.RootElement.ValueKind == JsonValueKind.Object &&
                   document.RootElement.TryGetProperty(LogoutEvent, out JsonElement logoutEvent) &&
                   logoutEvent.ValueKind == JsonValueKind.Object;
        }
        catch (JsonException)
        {
            return false;
        }
    }
}
