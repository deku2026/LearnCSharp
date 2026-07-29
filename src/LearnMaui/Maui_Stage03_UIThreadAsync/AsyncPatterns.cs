using System.Windows.Input;

namespace LearnMaui.Maui_Stage03_UIThreadAsync;

public sealed class AsyncWorkCoordinator : IDisposable
{
    private readonly SemaphoreSlim _singleFlight = new(1, 1);
    private CancellationTokenSource? _currentWork;

    public async Task<string> SearchAsync(string query, CancellationToken cancellationToken)
    {
        await _singleFlight.WaitAsync(cancellationToken);
        try
        {
            Cancel();
            _currentWork = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            await Task.Delay(TimeSpan.FromMilliseconds(180), _currentWork.Token);
            return $"{query}: {query.Length * 7} simulated results";
        }
        finally
        {
            _singleFlight.Release();
        }
    }

    public static Task<long> CountOnWorkerAsync(int upperBound, CancellationToken cancellationToken)
    {
        return Task.Run(() =>
        {
            long sum = 0;
            for (int index = 0; index <= upperBound; index++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                sum += index;
            }

            return sum;
        }, cancellationToken);
    }

    public static async Task<int> CountTimerTicksAsync(
        int count,
        TimeSpan interval,
        CancellationToken cancellationToken)
    {
        int ticks = 0;
        using PeriodicTimer timer = new(interval);
        while (ticks < count && await timer.WaitForNextTickAsync(cancellationToken))
        {
            ticks++;
        }

        return ticks;
    }

    public void Cancel()
    {
        _currentWork?.Cancel();
        _currentWork?.Dispose();
        _currentWork = null;
    }

    public void Dispose()
    {
        Cancel();
        _singleFlight.Dispose();
    }
}

public sealed class AsyncCommand(
    Func<CancellationToken, Task> execute,
    Func<bool>? canExecute = null) : ICommand, IDisposable
{
    private CancellationTokenSource? _execution;
    private bool _isRunning;

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter)
    {
        return !_isRunning && (canExecute?.Invoke() ?? true);
    }

    public async void Execute(object? parameter)
    {
        if (!CanExecute(parameter))
        {
            return;
        }

        _isRunning = true;
        _execution = new CancellationTokenSource();
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        try
        {
            await execute(_execution.Token);
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            _execution.Dispose();
            _execution = null;
            _isRunning = false;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public void Cancel()
    {
        _execution?.Cancel();
    }

    public void Dispose()
    {
        Cancel();
        _execution?.Dispose();
    }
}
