using System.Text.Json;

namespace Campus.Testing;

public static class ProblemDetailsAssertions
{
    public static async Task AssertErrorCodeAsync(HttpResponseMessage response, string expectedErrorCode)
    {
        await using Stream stream = await response.Content.ReadAsStreamAsync();
        using JsonDocument document = await JsonDocument.ParseAsync(stream);
        JsonElement doc = document.RootElement;

        if (doc.TryGetProperty("errorCode", out JsonElement direct))
        {
            if (!string.Equals(direct.GetString(), expectedErrorCode, StringComparison.Ordinal))
            {
                throw new InvalidOperationException($"Expected errorCode '{expectedErrorCode}', got '{direct.GetString()}'.");
            }

            return;
        }

        if (doc.TryGetProperty("extensions", out JsonElement ext) && ext.TryGetProperty("errorCode", out JsonElement nested))
        {
            if (!string.Equals(nested.GetString(), expectedErrorCode, StringComparison.Ordinal))
            {
                throw new InvalidOperationException($"Expected errorCode '{expectedErrorCode}', got '{nested.GetString()}'.");
            }

            return;
        }

        throw new InvalidOperationException("Response JSON missing errorCode.");
    }
}
