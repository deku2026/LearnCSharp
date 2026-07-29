namespace LearnMaui.Maui_Stage10_DevicePermissions;

public enum CapabilityOutcome
{
    Success,
    Unsupported,
    Disabled,
    Denied,
    Canceled,
    Failed,
}

public sealed record CapabilityResult<T>(
    CapabilityOutcome Outcome,
    T? Value = default,
    string? Message = null)
{
    public bool IsSuccess => Outcome == CapabilityOutcome.Success;
}

public sealed record DeviceSnapshot(
    string Model,
    string Platform,
    string Idiom,
    string NetworkAccess,
    string Profiles,
    double Density,
    double BatteryLevel,
    string BatteryState,
    bool HapticsSupported,
    bool VibrationSupported,
    string AppDataDirectory);

public interface IDeviceCapabilities
{
    DeviceSnapshot CaptureSnapshot();

    Task<PermissionStatus> GetLocationPermissionAsync(
        bool requestIfNeeded,
        CancellationToken cancellationToken);

    Task<CapabilityResult<Location>> GetLocationAsync(CancellationToken cancellationToken);

    Task<CapabilityResult<string>> RoundTripSecretAsync(
        string value,
        CancellationToken cancellationToken);
}

public sealed class MauiDeviceCapabilities : IDeviceCapabilities
{
    private const string SecretKey = "stage10_secret";

    public DeviceSnapshot CaptureSnapshot()
    {
        DisplayInfo display = DeviceDisplay.Current.MainDisplayInfo;
        return new DeviceSnapshot(
            DeviceInfo.Current.Model,
            DeviceInfo.Current.Platform.ToString(),
            DeviceInfo.Current.Idiom.ToString(),
            Connectivity.Current.NetworkAccess.ToString(),
            string.Join(", ", Connectivity.Current.ConnectionProfiles),
            display.Density,
            Battery.Default.ChargeLevel,
            Battery.Default.State.ToString(),
            HapticFeedback.Default.IsSupported,
            Vibration.Default.IsSupported,
            FileSystem.AppDataDirectory);
    }

    public async Task<PermissionStatus> GetLocationPermissionAsync(
        bool requestIfNeeded,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        PermissionStatus status =
            await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
        if (status != PermissionStatus.Granted && requestIfNeeded)
        {
            status = await MainThread.InvokeOnMainThreadAsync(
                Permissions.RequestAsync<Permissions.LocationWhenInUse>);
        }

        cancellationToken.ThrowIfCancellationRequested();
        return status;
    }

    public async Task<CapabilityResult<Location>> GetLocationAsync(
        CancellationToken cancellationToken)
    {
        try
        {
            PermissionStatus permission =
                await GetLocationPermissionAsync(true, cancellationToken);
            if (permission != PermissionStatus.Granted)
            {
                return new(CapabilityOutcome.Denied, Message: $"Permission={permission}");
            }

            if (!Geolocation.Default.IsEnabled)
            {
                return new(CapabilityOutcome.Disabled, Message: "Location services are disabled.");
            }

            GeolocationRequest request = new(
                GeolocationAccuracy.Medium,
                TimeSpan.FromSeconds(8));
            Location? location = await Geolocation.Default.GetLocationAsync(
                request,
                cancellationToken);
            return location is null
                ? new(CapabilityOutcome.Failed, Message: "No location was returned.")
                : new(CapabilityOutcome.Success, location);
        }
        catch (FeatureNotSupportedException exception)
        {
            return new(CapabilityOutcome.Unsupported, Message: exception.Message);
        }
        catch (OperationCanceledException)
        {
            return new(CapabilityOutcome.Canceled);
        }
        catch (Exception exception)
        {
            return new(CapabilityOutcome.Failed, Message: exception.Message);
        }
    }

    public async Task<CapabilityResult<string>> RoundTripSecretAsync(
        string value,
        CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            await SecureStorage.Default.SetAsync(SecretKey, value);
            string? restored = await SecureStorage.Default.GetAsync(SecretKey);
            cancellationToken.ThrowIfCancellationRequested();
            return new(CapabilityOutcome.Success, restored ?? string.Empty);
        }
        catch (OperationCanceledException)
        {
            return new(CapabilityOutcome.Canceled);
        }
        catch (Exception exception)
        {
            return new(CapabilityOutcome.Failed, Message: exception.Message);
        }
    }
}

public sealed class FakeDeviceCapabilities(DeviceSnapshot snapshot) : IDeviceCapabilities
{
    public PermissionStatus LocationPermission { get; set; } = PermissionStatus.Granted;

    public Location Location { get; set; } = new(31.2304, 121.4737);

    public DeviceSnapshot CaptureSnapshot()
    {
        return snapshot;
    }

    public Task<PermissionStatus> GetLocationPermissionAsync(
        bool requestIfNeeded,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(LocationPermission);
    }

    public Task<CapabilityResult<Location>> GetLocationAsync(
        CancellationToken cancellationToken)
    {
        return Task.FromResult(new CapabilityResult<Location>(
            CapabilityOutcome.Success,
            Location));
    }

    public Task<CapabilityResult<string>> RoundTripSecretAsync(
        string value,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(new CapabilityResult<string>(
            CapabilityOutcome.Success,
            value));
    }
}
