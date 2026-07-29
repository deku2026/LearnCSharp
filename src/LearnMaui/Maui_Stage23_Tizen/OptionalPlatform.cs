namespace LearnMaui.Maui_Stage23_Tizen;

public enum OptionalPlatformDecision
{
    NotApplicable,
    Spike,
    Adopt,
    Freeze,
}

public sealed record TizenBusinessCase(
    bool HasContractualTarget,
    bool HasSupportedDevice,
    bool HasSigningOwner,
    bool HasTvUxOwner,
    bool SharedCoreStable)
{
    public OptionalPlatformDecision Decide()
    {
        if (!HasContractualTarget)
        {
            return OptionalPlatformDecision.NotApplicable;
        }

        if (!HasSupportedDevice || !HasSigningOwner || !HasTvUxOwner || !SharedCoreStable)
        {
            return OptionalPlatformDecision.Spike;
        }

        return OptionalPlatformDecision.Adopt;
    }
}

public sealed record CapabilityStatus(
    string Name,
    bool Supported,
    string Fallback,
    string Evidence);

public interface IOptionalPlatformCapabilities
{
    string PlatformName { get; }

    IReadOnlyList<CapabilityStatus> Inspect();
}

public sealed class AndroidFallbackCapabilities : IOptionalPlatformCapabilities
{
    public string PlatformName => DeviceInfo.Current.Platform.ToString();

    public IReadOnlyList<CapabilityStatus> Inspect() =>
    [
        new("Network", true, "cached read model", "Connectivity.Current.NetworkAccess"),
        new("Pointer-free focus", true, "touch navigation", "semantic focus order"),
        new("Remote D-pad", false, "platform input adapter", "not a Tizen target"),
        new("TV signing", false, "Samsung certificate profile", "Tizen workload not installed"),
        new("Push/background", false, "poll on foreground", "must re-check target API"),
        new("Biometrics", false, "device-code authentication", "TV capability not assumed"),
    ];
}

public sealed record TvFocusNode(string Id, int Row, int Column, bool Enabled);

public sealed class TvFocusNavigator
{
    public TvFocusNode? Move(
        TvFocusNode current,
        IReadOnlyList<TvFocusNode> nodes,
        int rowDelta,
        int columnDelta)
    {
        ArgumentNullException.ThrowIfNull(current);
        ArgumentNullException.ThrowIfNull(nodes);

        return nodes
            .Where(node => node.Enabled && node != current)
            .Select(node => new
            {
                Node = node,
                Distance =
                    Math.Abs(node.Row - (current.Row + rowDelta)) +
                    Math.Abs(node.Column - (current.Column + columnDelta)),
                DirectionValid =
                    (rowDelta == 0 || Math.Sign(node.Row - current.Row) == Math.Sign(rowDelta)) &&
                    (columnDelta == 0 || Math.Sign(node.Column - current.Column) == Math.Sign(columnDelta)),
            })
            .Where(candidate => candidate.DirectionValid)
            .OrderBy(candidate => candidate.Distance)
            .ThenBy(candidate => candidate.Node.Row)
            .ThenBy(candidate => candidate.Node.Column)
            .Select(candidate => candidate.Node)
            .FirstOrDefault();
    }
}

public sealed record OptionalWorkloadManifest(
    string Workload,
    string SdkVersion,
    string EmulatorVersion,
    string TargetDevice,
    DateOnly VerifiedOn)
{
    public bool IsReproducible =>
        !string.IsNullOrWhiteSpace(Workload) &&
        !string.IsNullOrWhiteSpace(SdkVersion) &&
        !string.IsNullOrWhiteSpace(TargetDevice);
}
