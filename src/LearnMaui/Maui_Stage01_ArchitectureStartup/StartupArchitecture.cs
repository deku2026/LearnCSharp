using System.Collections.Concurrent;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace LearnMaui.Maui_Stage01_ArchitectureStartup;

public interface IStartupClock
{
    DateTimeOffset GetNow();
}

public sealed class SystemStartupClock(ILogger<SystemStartupClock> logger) : IStartupClock
{
    public DateTimeOffset GetNow()
    {
        DateTimeOffset now = DateTimeOffset.Now;
        logger.LogInformation("Startup clock read at {Now}", now);
        return now;
    }
}

public sealed class AppSession
{
    public Guid Id { get; } = Guid.NewGuid();
}

public sealed class StartupProbe
{
    public Guid Id { get; } = Guid.NewGuid();
}

public sealed class StartupLoggerProvider : ILoggerProvider
{
    private static readonly ConcurrentQueue<string> Entries = new();

    public static IReadOnlyCollection<string> Snapshot => [.. Entries];

    public ILogger CreateLogger(string categoryName)
    {
        return new StartupLogger(categoryName);
    }

    public void Dispose()
    {
    }

    private sealed class StartupLogger(string categoryName) : ILogger
    {
        public IDisposable? BeginScope<TState>(TState state)
            where TState : notnull
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= LogLevel.Information;
        }

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            string entry = $"{DateTimeOffset.Now:HH:mm:ss.fff} [{logLevel}] {categoryName}: {formatter(state, exception)}";
            Entries.Enqueue(entry);
            Debug.WriteLine(entry);
        }
    }
}
