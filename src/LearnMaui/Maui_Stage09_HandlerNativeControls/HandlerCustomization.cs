using Microsoft.Maui.Handlers;
#if ANDROID
using Android.Content.Res;
using Microsoft.Maui.Platform;
#endif

namespace LearnMaui.Maui_Stage09_HandlerNativeControls;

public sealed class TintedEntry : Entry
{
    public static readonly BindableProperty TintColorProperty = BindableProperty.Create(
        nameof(TintColor),
        typeof(Color),
        typeof(TintedEntry),
        Colors.DeepSkyBlue,
        propertyChanged: static (bindable, _, _) =>
            ((TintedEntry)bindable).Handler?.UpdateValue(nameof(TintColor)));

    public Color TintColor
    {
        get => (Color)GetValue(TintColorProperty);
        set => SetValue(TintColorProperty, value);
    }
}

public static class HandlerCustomization
{
    private const string GlobalEntryKey = "LearnMaui.Stage09.GlobalEntry";
    private static int _globalMapperRuns;

    public static int GlobalMapperRuns => Volatile.Read(ref _globalMapperRuns);

    public static void Configure(IMauiHandlersCollection handlers)
    {
        handlers.AddHandler<TintedEntry, AuditedEntryHandler>();
    }

    public static void ConfigureGlobalMappers()
    {
        EntryHandler.Mapper.AppendToMapping(GlobalEntryKey, static (handler, view) =>
        {
            Interlocked.Increment(ref _globalMapperRuns);
#if ANDROID
            handler.PlatformView.SetSelectAllOnFocus(true);
#endif
        });

        EntryHandler.Mapper.AppendToMapping(
            nameof(TintedEntry.TintColor),
            static (handler, view) =>
            {
                if (view is not TintedEntry tinted)
                {
                    return;
                }

#if ANDROID
                handler.PlatformView.BackgroundTintList =
                    ColorStateList.ValueOf(tinted.TintColor.ToPlatform());
#endif
            });
    }
}

public sealed class AuditedEntryHandler : EntryHandler
{
    private static int _connectedCount;
    private static int _disconnectedCount;

    public static int ConnectedCount => Volatile.Read(ref _connectedCount);

    public static int DisconnectedCount => Volatile.Read(ref _disconnectedCount);

#if ANDROID
    protected override void ConnectHandler(MauiAppCompatEditText platformView)
    {
        base.ConnectHandler(platformView);
        platformView.FocusChange += OnFocusChange;
        Interlocked.Increment(ref _connectedCount);
    }

    protected override void DisconnectHandler(MauiAppCompatEditText platformView)
    {
        platformView.FocusChange -= OnFocusChange;
        Interlocked.Increment(ref _disconnectedCount);
        base.DisconnectHandler(platformView);
    }

    private static void OnFocusChange(
        object? sender,
        Android.Views.View.FocusChangeEventArgs e)
    {
        if (e.HasFocus && sender is MauiAppCompatEditText entry)
        {
            entry.SetSelection(0, entry.Text?.Length ?? 0);
        }
    }
#endif
}

public sealed class HandlerLifetimeProbe : IDisposable
{
    private VisualElement? _element;

    public string PlatformViewType { get; private set; } = "not connected";

    public void Attach(VisualElement element)
    {
        Detach();
        _element = element;
        element.HandlerChanged += OnHandlerChanged;
        element.HandlerChanging += OnHandlerChanging;
        UpdatePlatformType();
    }

    public void Detach()
    {
        if (_element is null)
        {
            return;
        }

        _element.HandlerChanged -= OnHandlerChanged;
        _element.HandlerChanging -= OnHandlerChanging;
        _element = null;
        PlatformViewType = "detached";
    }

    private void OnHandlerChanged(object? sender, EventArgs e)
    {
        UpdatePlatformType();
    }

    private void OnHandlerChanging(object? sender, HandlerChangingEventArgs e)
    {
        PlatformViewType = e.OldHandler?.PlatformView?.GetType().FullName
            ?? "disconnecting";
    }

    private void UpdatePlatformType()
    {
        PlatformViewType = _element?.Handler?.PlatformView?.GetType().FullName
            ?? "not connected";
    }

    public void Dispose()
    {
        Detach();
    }
}
