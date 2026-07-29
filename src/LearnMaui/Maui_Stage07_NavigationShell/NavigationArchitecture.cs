using System.Collections.Concurrent;

namespace LearnMaui.Maui_Stage07_NavigationShell;

public static class RouteCatalog
{
    public const string Home = "home";
    public const string ArticleDetail = "article-detail";
    public const string Login = "login";
    public const string Settings = "settings";
}

public readonly record struct ArticleRoute(string Id)
{
    public static bool TryCreate(string? value, out ArticleRoute route)
    {
        route = default;
        if (string.IsNullOrWhiteSpace(value) ||
            value.Length > 64 ||
            !value.All(character =>
                char.IsLetterOrDigit(character) || character is '-' or '_'))
        {
            return false;
        }

        route = new ArticleRoute(value);
        return true;
    }
}

public sealed class SessionState
{
    public bool IsAuthenticated { get; set; }
}

public sealed class NavigationGate(SessionState session)
{
    private static readonly HashSet<string> ProtectedRoutes =
        [RouteCatalog.Settings];

    public bool CanNavigate(string route, out string? redirect)
    {
        bool protectedRoute = ProtectedRoutes.Contains(route);
        redirect = protectedRoute && !session.IsAuthenticated
            ? RouteCatalog.Login
            : null;
        return redirect is null;
    }
}

public sealed class NavigationJournal
{
    private readonly ConcurrentQueue<string> _entries = new();

    public IReadOnlyCollection<string> Snapshot => [.. _entries];

    public void Record(string operation)
    {
        _entries.Enqueue($"{DateTimeOffset.Now:HH:mm:ss.fff} {operation}");
    }
}

public interface INavigationService
{
    Task NavigateToArticleAsync(string id, CancellationToken cancellationToken);

    Task GoBackAsync(CancellationToken cancellationToken);
}

public sealed class ShellNavigationService(
    NavigationGate gate,
    NavigationJournal journal) : INavigationService
{
    public async Task NavigateToArticleAsync(string id, CancellationToken cancellationToken)
    {
        if (!ArticleRoute.TryCreate(id, out ArticleRoute route))
        {
            throw new ArgumentException("Article id is invalid.", nameof(id));
        }

        cancellationToken.ThrowIfCancellationRequested();
        if (!gate.CanNavigate(RouteCatalog.ArticleDetail, out string? redirect))
        {
            await Shell.Current.GoToAsync(redirect);
            return;
        }

        Dictionary<string, object> parameters = new()
        {
            [ArticleDetailPage.ArticleIdKey] = route.Id,
        };
        journal.Record($"GoTo {RouteCatalog.ArticleDetail} id={route.Id}");
        await Shell.Current.GoToAsync(RouteCatalog.ArticleDetail, parameters);
    }

    public async Task GoBackAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        journal.Record("GoTo ..");
        await Shell.Current.GoToAsync("..");
    }
}

public sealed class NavigationResultBroker<T>
{
    private TaskCompletionSource<T>? _pending;

    public Task<T> WaitAsync(CancellationToken cancellationToken)
    {
        _pending = new TaskCompletionSource<T>(
            TaskCreationOptions.RunContinuationsAsynchronously);
        cancellationToken.Register(() => _pending.TrySetCanceled(cancellationToken));
        return _pending.Task;
    }

    public bool TrySetResult(T result)
    {
        return _pending?.TrySetResult(result) ?? false;
    }
}
