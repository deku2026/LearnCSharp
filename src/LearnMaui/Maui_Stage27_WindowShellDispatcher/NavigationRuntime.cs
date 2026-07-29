using System.Collections.Concurrent;

namespace LearnMaui.Maui_Stage27_WindowShellDispatcher;

public static class RouteCatalog
{
    public const string Home = "home";
    public const string Detail = "detail";
    public const string Login = "login";
    public const string DetailId = "id";
}

public sealed class NavigationJournal
{
    private readonly ConcurrentQueue<string> _entries = new();

    public void Record(string value)
    {
        _entries.Enqueue($"{DateTimeOffset.Now:HH:mm:ss.fff} {value}");
        while (_entries.Count > 40)
        {
            _entries.TryDequeue(out _);
        }
    }

    public IReadOnlyList<string> Snapshot() => _entries.ToArray();
}

public sealed class WindowJournal
{
    private readonly ConcurrentQueue<string> _entries = new();

    public void Attach(Window window)
    {
        window.Created += (_, _) => Record("Created");
        window.Activated += (_, _) => Record("Activated");
        window.Deactivated += (_, _) => Record("Deactivated");
        window.Stopped += (_, _) => Record("Stopped");
        window.Resumed += (_, _) => Record("Resumed");
        window.Destroying += (_, _) => Record("Destroying");
    }

    public IReadOnlyList<string> Snapshot() => _entries.ToArray();

    private void Record(string value)
    {
        _entries.Enqueue($"{DateTimeOffset.Now:HH:mm:ss.fff} Window.{value}");
    }
}

public sealed class AppShell : Shell
{
    private readonly NavigationJournal _journal;

    public AppShell(MainPage mainPage, NavigationJournal journal)
    {
        _journal = journal;
        FlyoutBehavior = FlyoutBehavior.Disabled;
        Items.Add(new TabBar
        {
            Items =
            {
                new ShellContent
                {
                    Title = "专家导航",
                    Route = RouteCatalog.Home,
                    Content = mainPage,
                },
            },
        });

        Navigating += OnNavigating;
        Navigated += OnNavigated;
    }

    private void OnNavigating(object? sender, ShellNavigatingEventArgs e)
    {
        _journal.Record($"Navigating {e.Current?.Location} -> {e.Target.Location}");
    }

    private void OnNavigated(object? sender, ShellNavigatedEventArgs e)
    {
        _journal.Record($"Navigated source={e.Source}, location={e.Current.Location}");
    }
}

public sealed class DetailPage : ContentPage, IQueryAttributable
{
    private readonly Label _label = new()
    {
        Text = "等待 IQueryAttributable 参数",
        HorizontalOptions = LayoutOptions.Center,
        VerticalOptions = LayoutOptions.Center,
    };

    public DetailPage()
    {
        Title = "详情";
        Content = _label;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        string? raw = query.TryGetValue(RouteCatalog.DetailId, out object? value)
            ? value?.ToString()
            : null;
        _label.Text = DeepLinkRoute.IsValidId(raw)
            ? $"detail id={raw}"
            : "invalid detail id";
    }
}

public sealed class LoginPage : ContentPage
{
    public LoginPage(SessionGate gate)
    {
        Title = "登录门闸";
        Button button = new()
        {
            Text = "实验登录",
            AutomationId = "NavigationLab.Login",
        };
        button.Clicked += (_, _) =>
        {
            gate.SignIn();
            button.Text = "已登录，可继续原目标";
        };
        Content = button;
    }
}

public sealed record DeepLinkRoute(string DetailId)
{
    public static bool TryParse(Uri uri, out DeepLinkRoute route)
    {
        route = new DeepLinkRoute(string.Empty);
        if (uri.Scheme != "learnmaui" ||
            uri.Host != "detail")
        {
            return false;
        }

        string id = uri.AbsolutePath.Trim('/');
        if (!IsValidId(id))
        {
            return false;
        }

        route = new DeepLinkRoute(id);
        return true;
    }

    public static bool IsValidId(string? value) =>
        value is { Length: >= 1 and <= 64 } &&
        value.All(character =>
            char.IsAsciiLetterOrDigit(character) || character is '-' or '_');
}

public sealed class SessionGate
{
    public bool IsSignedIn { get; private set; }

    public void SignIn()
    {
        IsSignedIn = true;
    }

    public void SignOut()
    {
        IsSignedIn = false;
    }
}

public sealed record NavigationOutcome(
    bool Accepted,
    bool Duplicate,
    string Destination,
    string Reason);

public sealed class DeepLinkCoordinator(
    SessionGate sessionGate,
    NavigationJournal journal,
    IUiDispatcher dispatcher)
{
    private readonly SemaphoreSlim _navigationLock = new(1, 1);
    private readonly TaskCompletionSource _shellReady =
        new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly HashSet<string> _recentRoutes = new(StringComparer.Ordinal);
    private AppShell? _shell;

    public void AttachShell(AppShell shell)
    {
        _shell = shell;
    }

    public void MarkShellReady()
    {
        _shellReady.TrySetResult();
    }

    public async Task<NavigationOutcome> NavigateAsync(
        Uri uri,
        CancellationToken cancellationToken)
    {
        if (!DeepLinkRoute.TryParse(uri, out DeepLinkRoute route))
        {
            return new NavigationOutcome(false, false, string.Empty, "untrusted route");
        }

        await _shellReady.Task.WaitAsync(cancellationToken);
        await _navigationLock.WaitAsync(cancellationToken);
        try
        {
            string routeKey = $"detail/{route.DetailId}";
            if (!_recentRoutes.Add(routeKey))
            {
                return new NavigationOutcome(true, true, routeKey, "idempotent duplicate suppressed");
            }

            if (_recentRoutes.Count > 32)
            {
                _recentRoutes.Clear();
                _recentRoutes.Add(routeKey);
            }

            if (!sessionGate.IsSignedIn)
            {
                await dispatcher.InvokeAsync(() =>
                    _shell!.GoToAsync(RouteCatalog.Login));
                journal.Record($"DeepLink gated {routeKey} -> login");
                return new NavigationOutcome(true, false, RouteCatalog.Login, "authentication required");
            }

            await dispatcher.InvokeAsync(() =>
                _shell!.GoToAsync(
                    RouteCatalog.Detail,
                    new Dictionary<string, object>
                    {
                        [RouteCatalog.DetailId] = route.DetailId,
                    }));
            journal.Record($"DeepLink -> {routeKey}");
            return new NavigationOutcome(true, false, routeKey, "navigated");
        }
        finally
        {
            _navigationLock.Release();
        }
    }
}

public interface IUiDispatcher
{
    bool IsDispatchRequired { get; }

    Task InvokeAsync(Func<Task> action);
}

public sealed class MauiUiDispatcher : IUiDispatcher
{
    public bool IsDispatchRequired => !MainThread.IsMainThread;

    public Task InvokeAsync(Func<Task> action)
    {
        ArgumentNullException.ThrowIfNull(action);
        return MainThread.IsMainThread
            ? action()
            : MainThread.InvokeOnMainThreadAsync(action);
    }
}

public sealed class PageLifetime : IDisposable
{
    private CancellationTokenSource? _appearanceWork;

    public int AppearingCount { get; private set; }

    public int CancellationCount { get; private set; }

    public CancellationToken CurrentToken =>
        _appearanceWork?.Token ?? new CancellationToken(true);

    public void Appear()
    {
        Disappear();
        AppearingCount++;
        _appearanceWork = new CancellationTokenSource();
    }

    public void Disappear()
    {
        if (_appearanceWork is null)
        {
            return;
        }

        _appearanceWork.Cancel();
        _appearanceWork.Dispose();
        _appearanceWork = null;
        CancellationCount++;
    }

    public void Dispose()
    {
        Disappear();
    }
}
