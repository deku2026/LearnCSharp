using System.Collections.Concurrent;

namespace LearnMaui.Maui_Stage02_LifecycleWindowState;

public sealed class LifecycleJournal
{
    private static readonly ConcurrentQueue<string> Entries = new();

    public IReadOnlyCollection<string> Snapshot => [.. Entries];

    public static void RecordPlatform(string eventName)
    {
        Entries.Enqueue($"{DateTimeOffset.Now:HH:mm:ss.fff} [platform] {eventName}");
    }

    public void Record(string eventName)
    {
        Entries.Enqueue($"{DateTimeOffset.Now:HH:mm:ss.fff} [shared] {eventName}");
    }

    public void Attach(Window window)
    {
        window.Created += (_, _) => Record("Window.Created");
        window.Activated += (_, _) => Record("Window.Activated");
        window.Deactivated += (_, _) => Record("Window.Deactivated");
        window.Stopped += (_, _) => Record("Window.Stopped");
        window.Resumed += (_, _) => Record("Window.Resumed");
        window.Destroying += (_, _) => Record("Window.Destroying");
    }
}

public sealed class PageWorkController : IDisposable
{
    private CancellationTokenSource? _workCancellation;

    public async Task RestartAsync(TimeSpan duration)
    {
        Cancel();
        _workCancellation = new CancellationTokenSource();
        await Task.Delay(duration, _workCancellation.Token);
    }

    public void Cancel()
    {
        _workCancellation?.Cancel();
        _workCancellation?.Dispose();
        _workCancellation = null;
    }

    public void Dispose()
    {
        Cancel();
    }
}

public sealed class StateStore
{
    private const string DraftKey = "stage02_article_draft";

    public void SaveDraft(string draft)
    {
        Preferences.Default.Set(DraftKey, draft);
    }

    public string LoadDraft()
    {
        return Preferences.Default.Get(DraftKey, string.Empty);
    }
}

public static class DeepLinkParser
{
    public static bool TryParseArticle(Uri uri, out string id)
    {
        id = string.Empty;
        if (uri.Scheme is not ("https" or "learnmaui"))
        {
            return false;
        }

        string[] segments = uri.AbsolutePath
            .Trim('/')
            .Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (uri.Scheme == "learnmaui" && uri.Host == "articles")
        {
            segments = ["articles", .. segments];
        }

        bool trustedHost = uri.Scheme == "learnmaui" || uri.Host == "learnmaui.example";
        if (!trustedHost ||
            segments.Length != 2 ||
            segments[0] != "articles" ||
            segments[1].Length is < 1 or > 64 ||
            !segments[1].All(character =>
                char.IsLetterOrDigit(character) || character is '_' or '-'))
        {
            return false;
        }

        id = segments[1];
        return true;
    }
}
