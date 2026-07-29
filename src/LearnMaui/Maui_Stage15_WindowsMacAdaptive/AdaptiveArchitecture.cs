using System.Collections.Concurrent;

namespace LearnMaui.Maui_Stage15_WindowsMacAdaptive;

public enum AdaptiveMode
{
    Compact,
    Medium,
    Expanded,
}

public enum NavigationPresentation
{
    BottomTabs,
    CollapsedRail,
    ExpandedRail,
}

public sealed record AdaptiveLayoutDecision(
    AdaptiveMode Mode,
    NavigationPresentation Navigation,
    int ContentColumns,
    bool ShowDetailPane);

public static class AdaptiveLayoutPolicy
{
    public static AdaptiveLayoutDecision Evaluate(double width, double height)
    {
        double shortest = Math.Min(width, height);
        return width switch
        {
            < 600 => new(
                AdaptiveMode.Compact,
                NavigationPresentation.BottomTabs,
                1,
                false),
            < 1000 => new(
                AdaptiveMode.Medium,
                NavigationPresentation.CollapsedRail,
                shortest >= 600 ? 2 : 1,
                false),
            _ => new(
                AdaptiveMode.Expanded,
                NavigationPresentation.ExpandedRail,
                2,
                true),
        };
    }
}

public sealed class WindowWorkspace
{
    public required Guid Id { get; init; }

    public required WeakReference<Window> Window { get; init; }

    public string? SelectedEntityId { get; set; }

    public double Width { get; set; }

    public double Height { get; set; }
}

public sealed class WindowWorkspaceRegistry
{
    private readonly ConcurrentDictionary<Guid, WindowWorkspace> _workspaces = new();

    public IReadOnlyCollection<WindowWorkspace> Snapshot => [.. _workspaces.Values];

    public Guid Attach(Window window)
    {
        Guid id = Guid.NewGuid();
        WindowWorkspace workspace = new()
        {
            Id = id,
            Window = new WeakReference<Window>(window),
            Width = window.Width,
            Height = window.Height,
        };
        _workspaces[id] = workspace;
        window.SizeChanged += OnSizeChanged;
        window.Destroying += OnDestroying;
        return id;

        void OnSizeChanged(object? sender, EventArgs args)
        {
            workspace.Width = window.Width;
            workspace.Height = window.Height;
        }

        void OnDestroying(object? sender, EventArgs args)
        {
            window.SizeChanged -= OnSizeChanged;
            window.Destroying -= OnDestroying;
            _workspaces.TryRemove(id, out _);
        }
    }

    public Window CreateDetailWindow(string entityId)
    {
        ContentPage page = new()
        {
            Title = $"Detail {entityId}",
            Content = new Label
            {
                Text = $"Independent detail workspace: {entityId}",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
            },
        };
        Window window = new(page) { Title = $"Detail {entityId}" };
        Guid id = Attach(window);
        _workspaces[id].SelectedEntityId = entityId;
        return window;
    }
}

public interface IDesktopPlatformService
{
    string Platform { get; }

    string DescribeWindowCapabilities();
}

public sealed class DesktopPlatformService : IDesktopPlatformService
{
    public string Platform => DeviceInfo.Current.Platform.ToString();

    public string DescribeWindowCapabilities()
    {
#if WINDOWS
        return "Windows: AppWindow presenter, minimize/maximize controls and multiple top-level windows.";
#elif MACCATALYST
        return "Mac Catalyst: scene/window lifecycle, menu bar and native pointer conventions.";
#else
        return "Android build: adaptive width and multi-window concepts are shared; desktop-native APIs are excluded.";
#endif
    }
}

public static class DesktopMenuFactory
{
    public static MenuBarItem Create()
    {
        MenuBarItem file = new() { Text = "File" };
        file.Add(new MenuFlyoutItem
        {
            Text = "New workspace",
            Command = new Command(() => { }),
        });
        file.Add(new MenuFlyoutItem
        {
            Text = "Close",
            Command = new Command(() => { }),
        });
        return file;
    }

    public static View CreatePointerTarget()
    {
        Border target = new()
        {
            Padding = 16,
            Content = new Label { Text = "Pointer and touch target" },
        };
        ToolTipProperties.SetText(target, "Available with mouse, touch and keyboard");
        target.GestureRecognizers.Add(new TapGestureRecognizer());
        target.GestureRecognizers.Add(new PointerGestureRecognizer());
        return target;
    }
}
