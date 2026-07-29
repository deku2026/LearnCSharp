using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace LearnMaui.Maui_Stage28_CollectionViewRuntime;

public sealed record RuntimeSampleItem(
    string Id,
    int Sequence,
    string Title,
    string Detail);

public sealed class ResettableObservableCollection<T> : ObservableCollection<T>
{
    public void ReplaceAll(IEnumerable<T> values)
    {
        ArgumentNullException.ThrowIfNull(values);

        Items.Clear();
        foreach (T value in values)
        {
            Items.Add(value);
        }

        OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
        OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
        OnCollectionChanged(
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }
}

public sealed record VisibleRange(
    int First,
    int Center,
    int Last,
    double VerticalDelta,
    DateTimeOffset ObservedAt);

public interface ICollectionDispatcher
{
    Task InvokeAsync(Action action);
}

public sealed class MauiCollectionDispatcher : ICollectionDispatcher
{
    public Task InvokeAsync(Action action)
    {
        ArgumentNullException.ThrowIfNull(action);
        if (MainThread.IsMainThread)
        {
            action();
            return Task.CompletedTask;
        }

        return MainThread.InvokeOnMainThreadAsync(action);
    }
}

public sealed class SafeAsyncCommand : ICommand
{
    private readonly Func<Task> _execute;
    private bool _running;

    public SafeAsyncCommand(Func<Task> execute)
    {
        _execute = execute;
    }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) => !_running;

    public async void Execute(object? parameter)
    {
        if (_running)
        {
            return;
        }

        _running = true;
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        try
        {
            await _execute();
        }
        finally
        {
            _running = false;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}

public sealed class CollectionLabViewModel : INotifyPropertyChanged
{
    private readonly ICollectionDispatcher _dispatcher;
    private readonly SemaphoreSlim _loadGate = new(1, 1);
    private int _nextSequence = 1;
    private int _page;
    private bool _isRefreshing;
    private VisibleRange? _visibleRange;
    private string _lastOperation = "seeded";

    public CollectionLabViewModel(ICollectionDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
        Items = new ResettableObservableCollection<RuntimeSampleItem>();
        Items.ReplaceAll(CreateItems(24, "initial"));
        RefreshCommand = new SafeAsyncCommand(
            () => RefreshAsync(CancellationToken.None));
        LoadMoreCommand = new SafeAsyncCommand(
            () => LoadMoreAsync(CancellationToken.None));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ResettableObservableCollection<RuntimeSampleItem> Items { get; }

    public ICommand RefreshCommand { get; }

    public ICommand LoadMoreCommand { get; }

    public bool IsRefreshing
    {
        get => _isRefreshing;
        private set => SetProperty(ref _isRefreshing, value);
    }

    public VisibleRange? VisibleRange
    {
        get => _visibleRange;
        private set => SetProperty(ref _visibleRange, value);
    }

    public string LastOperation
    {
        get => _lastOperation;
        private set => SetProperty(ref _lastOperation, value);
    }

    public async Task LoadMoreAsync(CancellationToken cancellationToken)
    {
        if (!await _loadGate.WaitAsync(0, cancellationToken))
        {
            LastOperation = "duplicate pagination suppressed";
            return;
        }

        try
        {
            int requestedPage = ++_page;
            await Task.Delay(120, cancellationToken);
            RuntimeSampleItem[] next = CreateItems(12, $"page-{requestedPage}");
            await _dispatcher.InvokeAsync(() =>
            {
                foreach (RuntimeSampleItem item in next)
                {
                    Items.Add(item);
                }

                LastOperation = $"page {requestedPage}; count={Items.Count}";
            });
        }
        finally
        {
            _loadGate.Release();
        }
    }

    public async Task RefreshAsync(CancellationToken cancellationToken)
    {
        await _dispatcher.InvokeAsync(() => IsRefreshing = true);
        try
        {
            await Task.Delay(120, cancellationToken);
            RuntimeSampleItem[] replacement = CreateItems(24, "refresh");
            await _dispatcher.InvokeAsync(() =>
            {
                Items.ReplaceAll(replacement);
                _page = 0;
                LastOperation = $"refreshed; count={Items.Count}";
            });
        }
        finally
        {
            await _dispatcher.InvokeAsync(() => IsRefreshing = false);
        }
    }

    public void TrackVisibleRange(
        int first,
        int center,
        int last,
        double verticalDelta)
    {
        VisibleRange = new VisibleRange(
            first,
            center,
            last,
            verticalDelta,
            DateTimeOffset.UtcNow);
    }

    private RuntimeSampleItem[] CreateItems(int count, string source)
    {
        RuntimeSampleItem[] items = new RuntimeSampleItem[count];
        for (int index = 0; index < count; index++)
        {
            int sequence = _nextSequence++;
            items[index] = new RuntimeSampleItem(
                $"item-{sequence}",
                sequence,
                $"Runtime sample {sequence}",
                $"{source}: stable id + compiled item binding");
        }

        return items;
    }

    private void SetProperty<T>(
        ref T storage,
        T value,
        [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(storage, value))
        {
            return;
        }

        storage = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
