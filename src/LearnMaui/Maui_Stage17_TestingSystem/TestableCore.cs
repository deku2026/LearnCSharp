using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LearnMaui.Maui_Stage17_TestingSystem;

public enum LoadState
{
    Idle,
    Loading,
    Content,
    Error,
}

public interface IDataGateway
{
    Task<string> GetAsync(CancellationToken cancellationToken);
}

public sealed class FakeDataGateway : IDataGateway
{
    public string Value { get; set; } = "test data";

    public Exception? Exception { get; set; }

    public TimeSpan Delay { get; set; }

    public async Task<string> GetAsync(CancellationToken cancellationToken)
    {
        if (Delay > TimeSpan.Zero)
        {
            await Task.Delay(Delay, cancellationToken);
        }

        if (Exception is not null)
        {
            throw Exception;
        }

        return Value;
    }
}

public interface IClock
{
    DateTimeOffset UtcNow { get; }
}

public sealed class FakeClock(DateTimeOffset utcNow) : IClock
{
    public DateTimeOffset UtcNow { get; set; } = utcNow;
}

public sealed class LoadViewModel(
    IDataGateway gateway,
    IClock clock) : INotifyPropertyChanged
{
    private LoadState _state;
    private string? _value;
    private string? _error;

    public event PropertyChangedEventHandler? PropertyChanged;

    public LoadState State
    {
        get => _state;
        private set => Set(ref _state, value);
    }

    public string? Value
    {
        get => _value;
        private set => Set(ref _value, value);
    }

    public string? Error
    {
        get => _error;
        private set => Set(ref _error, value);
    }

    public DateTimeOffset? LastLoadedAt { get; private set; }

    public async Task LoadAsync(CancellationToken cancellationToken)
    {
        State = LoadState.Loading;
        Error = null;
        try
        {
            Value = await gateway.GetAsync(cancellationToken);
            LastLoadedAt = clock.UtcNow;
            State = LoadState.Content;
        }
        catch (OperationCanceledException)
        {
            State = LoadState.Idle;
            throw;
        }
        catch (Exception exception)
        {
            Error = exception.Message;
            State = LoadState.Error;
        }
    }

    private void Set<T>(ref T field, T value, [CallerMemberName] string? name = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return;
        }

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

public sealed record TestCase(string Name, Func<CancellationToken, Task> ExecuteAsync);

public sealed record TestResult(
    string Name,
    bool Passed,
    TimeSpan Duration,
    string? Failure);

public sealed class ExecutableTestSuite
{
    public IReadOnlyList<TestCase> Cases { get; } =
    [
        new("VM success contract", TestSuccessAsync),
        new("VM error contract", TestErrorAsync),
        new("VM cancellation contract", TestCancellationAsync),
        new("INPC contract", TestPropertyChangedAsync),
        new("Clock determinism", TestClockAsync),
    ];

    public async Task<IReadOnlyList<TestResult>> RunAsync(
        CancellationToken cancellationToken)
    {
        List<TestResult> results = [];
        foreach (TestCase test in Cases)
        {
            cancellationToken.ThrowIfCancellationRequested();
            long started = Environment.TickCount64;
            try
            {
                await test.ExecuteAsync(cancellationToken);
                results.Add(new TestResult(
                    test.Name,
                    true,
                    TimeSpan.FromMilliseconds(Environment.TickCount64 - started),
                    null));
            }
            catch (Exception exception)
            {
                results.Add(new TestResult(
                    test.Name,
                    false,
                    TimeSpan.FromMilliseconds(Environment.TickCount64 - started),
                    exception.Message));
            }
        }

        return results;
    }

    private static async Task TestSuccessAsync(CancellationToken cancellationToken)
    {
        FakeClock clock = new(new DateTimeOffset(2026, 7, 30, 0, 0, 0, TimeSpan.Zero));
        LoadViewModel viewModel = new(new FakeDataGateway { Value = "ok" }, clock);
        await viewModel.LoadAsync(cancellationToken);
        Expect.Equal(LoadState.Content, viewModel.State);
        Expect.Equal("ok", viewModel.Value);
        Expect.Equal(clock.UtcNow, viewModel.LastLoadedAt);
    }

    private static async Task TestErrorAsync(CancellationToken cancellationToken)
    {
        LoadViewModel viewModel = new(
            new FakeDataGateway { Exception = new InvalidOperationException("offline") },
            new FakeClock(DateTimeOffset.UnixEpoch));
        await viewModel.LoadAsync(cancellationToken);
        Expect.Equal(LoadState.Error, viewModel.State);
        Expect.Equal("offline", viewModel.Error);
    }

    private static async Task TestCancellationAsync(CancellationToken cancellationToken)
    {
        using CancellationTokenSource canceled =
            CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        canceled.Cancel();
        LoadViewModel viewModel = new(
            new FakeDataGateway { Delay = TimeSpan.FromSeconds(1) },
            new FakeClock(DateTimeOffset.UnixEpoch));
        try
        {
            await viewModel.LoadAsync(canceled.Token);
            throw new TestFailureException("Expected cancellation.");
        }
        catch (OperationCanceledException)
        {
            Expect.Equal(LoadState.Idle, viewModel.State);
        }
    }

    private static async Task TestPropertyChangedAsync(CancellationToken cancellationToken)
    {
        LoadViewModel viewModel = new(
            new FakeDataGateway(),
            new FakeClock(DateTimeOffset.UnixEpoch));
        List<string?> changed = [];
        viewModel.PropertyChanged += (_, args) => changed.Add(args.PropertyName);
        await viewModel.LoadAsync(cancellationToken);
        Expect.Contains(nameof(LoadViewModel.State), changed);
        Expect.Contains(nameof(LoadViewModel.Value), changed);
    }

    private static async Task TestClockAsync(CancellationToken cancellationToken)
    {
        DateTimeOffset expected =
            new(2030, 1, 2, 3, 4, 5, TimeSpan.Zero);
        LoadViewModel viewModel = new(
            new FakeDataGateway(),
            new FakeClock(expected));
        await viewModel.LoadAsync(cancellationToken);
        Expect.Equal(expected, viewModel.LastLoadedAt);
    }
}

public sealed class TestFailureException(string message) : Exception(message);

public static class Expect
{
    public static void Equal<T>(T expected, T actual)
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual))
        {
            throw new TestFailureException($"Expected {expected}; actual {actual}.");
        }
    }

    public static void Contains<T>(T expected, IEnumerable<T> values)
    {
        if (!values.Contains(expected))
        {
            throw new TestFailureException($"Collection did not contain {expected}.");
        }
    }
}

public sealed class LearningPageObject
{
    public const string Query = "Stage17Query";
    public const string Load = "Stage17Load";
    public const string Result = "Stage17Result";
}
