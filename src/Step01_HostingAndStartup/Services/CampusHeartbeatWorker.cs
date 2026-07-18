namespace Step01_HostingAndStartup.Services;

/// <summary>
/// BackgroundService lab: PeriodicTimer, stoppingToken, try/catch, IServiceScopeFactory for scoped work.
/// </summary>
public sealed class CampusHeartbeatWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<CampusHeartbeatWorker> _logger;

    public CampusHeartbeatWorker(
        IServiceScopeFactory scopeFactory,
        IConfiguration configuration,
        ILogger<CampusHeartbeatWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _configuration = configuration;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Yield early so other hosted services (including Kestrel) can start.
        await Task.Yield();

        var seconds = _configuration.GetValue("Heartbeat:IntervalSeconds", 10);
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(seconds));

        _logger.LogInformation(
            "CampusHeartbeatWorker started. Interval={IntervalSeconds}s. Will stop when host stops.",
            seconds);

        try
        {
            while (!stoppingToken.IsCancellationRequested
                   && await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    // Pitfall #1: never inject scoped services into singleton hosted services.
                    // Open a scope per work unit instead.
                    using var scope = _scopeFactory.CreateScope();
                    var work = scope.ServiceProvider.GetRequiredService<ScopedWorkItem>();
                    await work.TickAsync(stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    // Pitfall #2: unhandled exceptions default to StopHost (.NET 6+).
                    _logger.LogError(ex, "Heartbeat work item failed; continuing loop.");
                }
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            // Expected on graceful shutdown
        }

        _logger.LogInformation("CampusHeartbeatWorker stopped gracefully.");
    }
}

/// <summary>Scoped dependency used only inside a per-tick scope.</summary>
public sealed class ScopedWorkItem(ILogger<ScopedWorkItem> logger, StartupProbe probe)
{
    public Task TickAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var n = probe.IncrementAndGet();
        logger.LogInformation("Heartbeat tick #{Tick} at {Time:O} (scoped work item instance)", n, DateTimeOffset.UtcNow);
        return Task.CompletedTask;
    }
}

/// <summary>Singleton counter shared across ticks for observability in tests/logs.</summary>
public sealed class StartupProbe
{
    private int _ticks;

    public int IncrementAndGet() => Interlocked.Increment(ref _ticks);

    public int Current => Volatile.Read(ref _ticks);
}
