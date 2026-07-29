using System.Security.Cryptography;

namespace LearnMaui.Maui_Stage20_PackagingCI;

public readonly record struct ReleaseVersion(
    int Major,
    int Minor,
    int Patch,
    int Build)
{
    public string Display => $"{Major}.{Minor}.{Patch}";

    public static bool TryParse(
        string displayVersion,
        string buildVersion,
        out ReleaseVersion version)
    {
        version = default;
        string[] parts = displayVersion.Split('.');
        if (parts.Length != 3 ||
            !int.TryParse(parts[0], out int major) ||
            !int.TryParse(parts[1], out int minor) ||
            !int.TryParse(parts[2], out int patch) ||
            !int.TryParse(buildVersion, out int build) ||
            major < 0 ||
            minor < 0 ||
            patch < 0 ||
            build <= 0)
        {
            return false;
        }

        version = new ReleaseVersion(major, minor, patch, build);
        return true;
    }
}

public sealed record SigningReadiness(
    bool IsReady,
    IReadOnlyList<string> MissingVariables)
{
    private static readonly string[] RequiredVariables =
    [
        "ANDROID_KEYSTORE_PATH",
        "ANDROID_KEYSTORE_PASSWORD",
        "ANDROID_KEY_ALIAS",
        "ANDROID_KEY_PASSWORD",
    ];

    public static SigningReadiness Inspect()
    {
        string[] missing = RequiredVariables
            .Where(name => string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(name)))
            .ToArray();
        return new SigningReadiness(missing.Length == 0, missing);
    }
}

public enum ReleaseChannel
{
    Internal,
    Beta,
    Production,
}

public sealed record RolloutPlan(
    ReleaseChannel Channel,
    int Percentage,
    bool HaltOnCrashRegression,
    bool HaltOnAnrRegression)
{
    public bool IsValid =>
        Percentage is >= 1 and <= 100 &&
        (Channel != ReleaseChannel.Production || HaltOnCrashRegression);
}

public sealed record ArtifactManifest(
    string FileName,
    long Bytes,
    string Sha256,
    string Commit,
    DateTimeOffset CreatedAt);

public static class ArtifactInspector
{
    public static async Task<ArtifactManifest> CreateManifestAsync(
        string path,
        string commit,
        CancellationToken cancellationToken)
    {
        await using FileStream stream = new(
            path,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            81920,
            FileOptions.Asynchronous | FileOptions.SequentialScan);
        byte[] hash = await SHA256.HashDataAsync(stream, cancellationToken);
        FileInfo file = new(path);
        return new ArtifactManifest(
            file.Name,
            file.Length,
            Convert.ToHexString(hash).ToLowerInvariant(),
            commit,
            DateTimeOffset.UtcNow);
    }
}

public static class ArtifactNaming
{
    public static string Create(
        string app,
        ReleaseVersion version,
        string runtimeIdentifier,
        string extension)
    {
        string safeExtension = extension.TrimStart('.');
        return $"{app}-{version.Display}+{version.Build}-{runtimeIdentifier}.{safeExtension}";
    }
}
