namespace Step01_HostingAndStartup.Services;

/// <summary>
/// IHostedService that hooks IHostApplicationLifetime events (Started / Stopping / Stopped).
/// </summary>
public sealed class LifetimeEventsLogger : IHostedService
{
    private readonly IHostApplicationLifetime _lifetime;
    private readonly ILogger<LifetimeEventsLogger> _logger;
    private CancellationTokenRegistration _started;
    private CancellationTokenRegistration _stopping;
    private CancellationTokenRegistration _stopped;

    public LifetimeEventsLogger(IHostApplicationLifetime lifetime, ILogger<LifetimeEventsLogger> logger)
    {
        _lifetime = lifetime;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Keep StartAsync short — hosted services start sequentially.
        _started = _lifetime.ApplicationStarted.Register(() =>
            _logger.LogInformation("Lifetime: ApplicationStarted (Kestrel is up, accepting requests)"));

        _stopping = _lifetime.ApplicationStopping.Register(() =>
            _logger.LogInformation("Lifetime: ApplicationStopping (graceful shutdown begun)"));

        _stopped = _lifetime.ApplicationStopped.Register(() =>
            _logger.LogInformation("Lifetime: ApplicationStopped (all hosted services stopped)"));

        _logger.LogInformation("LifetimeEventsLogger registered lifetime callbacks.");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _started.Dispose();
        _stopping.Dispose();
        _stopped.Dispose();
        _logger.LogInformation("LifetimeEventsLogger cleaned up.");
        return Task.CompletedTask;
    }
}
