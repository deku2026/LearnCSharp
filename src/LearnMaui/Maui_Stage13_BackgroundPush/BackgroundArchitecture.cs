using System.Collections.Concurrent;
#if ANDROID
using Android.App;
#endif

namespace LearnMaui.Maui_Stage13_BackgroundPush;

public enum NetworkConstraint
{
    Any,
    Connected,
    Unmetered,
}

public sealed record BackgroundWorkIntent(
    string UniqueName,
    string Reason,
    DateTimeOffset EarliestStart,
    NetworkConstraint Network,
    bool RequiresCharging);

public interface IBackgroundScheduler
{
    Task ScheduleAsync(
        BackgroundWorkIntent intent,
        CancellationToken cancellationToken);

    IReadOnlyCollection<BackgroundWorkIntent> Snapshot { get; }
}

public sealed class InMemoryBackgroundScheduler : IBackgroundScheduler
{
    private readonly ConcurrentDictionary<string, BackgroundWorkIntent> _scheduled =
        new(StringComparer.Ordinal);

    public IReadOnlyCollection<BackgroundWorkIntent> Snapshot =>
        [.. _scheduled.Values.OrderBy(value => value.EarliestStart)];

    public Task ScheduleAsync(
        BackgroundWorkIntent intent,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _scheduled[intent.UniqueName] = intent;
        return Task.CompletedTask;
    }
}

public sealed record SyncRunResult(
    bool DidRun,
    bool ShouldRetry,
    string Message,
    DateTimeOffset CompletedAt);

public sealed class BackgroundSyncCoordinator
{
    private readonly SemaphoreSlim _singleFlight = new(1, 1);
    private int _runCount;

    public int RunCount => Volatile.Read(ref _runCount);

    public async Task<SyncRunResult> RunAsync(
        string reason,
        bool networkAvailable,
        CancellationToken cancellationToken)
    {
        if (!await _singleFlight.WaitAsync(0, cancellationToken))
        {
            return new(false, false, "A sync run is already active.", DateTimeOffset.UtcNow);
        }

        try
        {
            if (!networkAvailable)
            {
                return new(false, true, "Network unavailable; scheduler may retry.", DateTimeOffset.UtcNow);
            }

            await Task.Delay(120, cancellationToken);
            Interlocked.Increment(ref _runCount);
            return new(true, false, $"Sync completed for {reason}.", DateTimeOffset.UtcNow);
        }
        finally
        {
            _singleFlight.Release();
        }
    }
}

public sealed record NotificationEnvelope(
    string Id,
    string Title,
    string Body,
    string Route,
    string? EntityId);

public readonly record struct NotificationRoute(string Route, string? EntityId);

public static class NotificationRouter
{
    private static readonly HashSet<string> AllowedRoutes =
        ["home", "article", "settings"];

    public static bool TryParse(
        NotificationEnvelope envelope,
        out NotificationRoute route)
    {
        route = default;
        if (!AllowedRoutes.Contains(envelope.Route) ||
            envelope.Id.Length is < 1 or > 128 ||
            (envelope.EntityId is { Length: > 64 }) ||
            (envelope.EntityId is not null &&
             !envelope.EntityId.All(character =>
                 char.IsLetterOrDigit(character) || character is '-' or '_')))
        {
            return false;
        }

        route = new NotificationRoute(envelope.Route, envelope.EntityId);
        return true;
    }
}

public interface INotificationPlatformBridge
{
    string Platform { get; }

    Task<string> EnsureDefaultChannelAsync(CancellationToken cancellationToken);
}

public sealed class AndroidNotificationPlatformBridge : INotificationPlatformBridge
{
    private const string ChannelId = "learnmaui_updates";

    public string Platform => DeviceInfo.Current.Platform.ToString();

    public Task<string> EnsureDefaultChannelAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
#if ANDROID
        if (OperatingSystem.IsAndroidVersionAtLeast(26))
        {
            NotificationChannel channel = new(
                ChannelId,
                "Learning updates",
                NotificationImportance.Default)
            {
                Description = "Background synchronization and learning updates",
            };
            NotificationManager manager =
                (NotificationManager)Android.App.Application.Context.GetSystemService(
                    Android.Content.Context.NotificationService)!;
            manager.CreateNotificationChannel(channel);
        }
#endif
        return Task.FromResult(ChannelId);
    }
}

public sealed class PushRegistrationStore
{
    public string? CurrentToken { get; private set; }

    public int Revision { get; private set; }

    public bool Update(string token)
    {
        if (string.IsNullOrWhiteSpace(token) || token == CurrentToken)
        {
            return false;
        }

        CurrentToken = token;
        Revision++;
        return true;
    }

    public void Clear()
    {
        CurrentToken = null;
        Revision++;
    }
}
