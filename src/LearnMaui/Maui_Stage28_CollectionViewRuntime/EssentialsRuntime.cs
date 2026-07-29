using System.Runtime;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace LearnMaui.Maui_Stage28_CollectionViewRuntime;

public sealed record EssentialsSnapshot(
    string Platform,
    string Idiom,
    string Device,
    string NetworkAccess,
    string LocationPermission,
    bool SecureStorageRoundTrip,
    string LastKnownLocation);

public sealed class EssentialsProbe
{
    private const string ProbeKey = "stage28_secure_storage_probe";

    public async Task<EssentialsSnapshot> InspectAsync(
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        PermissionStatus permission;
        try
        {
            permission =
                await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
        }
        catch (Exception exception) when (
            exception is FeatureNotSupportedException or PermissionException)
        {
            permission = PermissionStatus.Unknown;
        }

        bool secureRoundTrip = await ProbeSecureStorageAsync();
        string location = await GetLastKnownLocationAsync();
        return new EssentialsSnapshot(
            DeviceInfo.Current.Platform.ToString(),
            DeviceInfo.Current.Idiom.ToString(),
            DeviceInfo.Current.Name,
            Connectivity.Current.NetworkAccess.ToString(),
            permission.ToString(),
            secureRoundTrip,
            location);
    }

    private static async Task<bool> ProbeSecureStorageAsync()
    {
        string marker = Guid.NewGuid().ToString("N");
        try
        {
            await SecureStorage.Default.SetAsync(ProbeKey, marker);
            string? read = await SecureStorage.Default.GetAsync(ProbeKey);
            return CryptographicOperations.FixedTimeEquals(
                System.Text.Encoding.UTF8.GetBytes(marker),
                System.Text.Encoding.UTF8.GetBytes(read ?? string.Empty));
        }
        catch (Exception exception) when (
            exception is FeatureNotSupportedException or InvalidOperationException)
        {
            return false;
        }
        finally
        {
            SecureStorage.Default.Remove(ProbeKey);
        }
    }

    private static async Task<string> GetLastKnownLocationAsync()
    {
        try
        {
            Location? location = await Geolocation.Default.GetLastKnownLocationAsync();
            return location is null
                ? "none"
                : $"{location.Latitude:0.0000},{location.Longitude:0.0000}";
        }
        catch (Exception exception) when (
            exception is FeatureNotSupportedException or
                FeatureNotEnabledException or
                PermissionException)
        {
            return exception.GetType().Name;
        }
    }
}

public sealed record RuntimeSnapshot(
    string Framework,
    string RuntimeFlavor,
    Architecture ProcessArchitecture,
    bool DynamicCodeSupported,
    bool DynamicCodeCompiled,
    bool ServerGc,
    int GcGeneration,
    long ManagedBytes,
    string Tfm);

public static class RuntimeInspector
{
    public static RuntimeSnapshot Capture()
    {
        string runtimeFlavor = Type.GetType("Mono.Runtime") is null
            ? "CoreCLR/other"
            : "Mono";
        object allocationProbe = new();
        return new RuntimeSnapshot(
            RuntimeInformation.FrameworkDescription,
            runtimeFlavor,
            RuntimeInformation.ProcessArchitecture,
            RuntimeFeature.IsDynamicCodeSupported,
            RuntimeFeature.IsDynamicCodeCompiled,
            GCSettings.IsServerGC,
            GC.GetGeneration(allocationProbe),
            GC.GetTotalMemory(false),
            AppContext.TargetFrameworkName ?? "unknown");
    }
}

public sealed record CollectionPipelineStep(
    int Order,
    string Layer,
    string Responsibility);

public static class CollectionPipeline
{
    public static IReadOnlyList<CollectionPipelineStep> Describe() =>
    [
        new(1, "INotifyCollectionChanged", "add/remove/replace/reset notification"),
        new(2, "ItemsView/TemplatedItems", "materialize item and template contract"),
        new(3, "CollectionViewHandler", "map virtual list to platform controller"),
        new(4, "Android RecyclerView", "reuse holders and request child measure"),
        new(5, "Layout manager", "compute visible range and scrolling"),
        new(6, "Handler tree", "bind/rebind item views and native properties"),
    ];
}
