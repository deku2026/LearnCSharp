using System.Collections.Concurrent;
using Microsoft.Maui.Handlers;
#if ANDROID
using Android.Content.Res;
using Microsoft.Maui.Platform;
#endif

namespace LearnMaui.Maui_Stage26_HandlerMapperLayout;

public sealed class ExpertEntry : Entry
{
    public static readonly BindableProperty AccentColorProperty = BindableProperty.Create(
        nameof(AccentColor),
        typeof(Color),
        typeof(ExpertEntry),
        Colors.MediumPurple,
        propertyChanged: static (bindable, _, _) =>
            ((ExpertEntry)bindable).Handler?.UpdateValue(nameof(AccentColor)));

    public Color AccentColor
    {
        get => (Color)GetValue(AccentColorProperty);
        set => SetValue(AccentColorProperty, value);
    }

    public void RequestSelectAll()
    {
        Handler?.Invoke(HandlerLabMappings.SelectAllCommand, null);
    }
}

public static class MapperTrace
{
    private static readonly ConcurrentQueue<string> Events = new();

    public static void Record(string value)
    {
        Events.Enqueue($"{DateTimeOffset.UtcNow:HH:mm:ss.fff} {value}");
        while (Events.Count > 32)
        {
            Events.TryDequeue(out _);
        }
    }

    public static IReadOnlyList<string> Snapshot() => Events.ToArray();

    public static void Clear()
    {
        while (Events.TryDequeue(out _))
        {
        }
    }
}

public static class HandlerLabMappings
{
    private const string AccentKey = nameof(ExpertEntry.AccentColor);
    public const string SelectAllCommand = "LearnMaui.Stage26.SelectAll";
    private static int _configured;

    public static void Configure()
    {
        if (Interlocked.Exchange(ref _configured, 1) != 0)
        {
            return;
        }

        EntryHandler.Mapper.AppendToMapping(
            AccentKey,
            static (handler, view) =>
            {
                if (view is not ExpertEntry expert)
                {
                    return;
                }

                MapperTrace.Record("property mapper: AccentColor");
#if ANDROID
                handler.PlatformView.BackgroundTintList =
                    ColorStateList.ValueOf(expert.AccentColor.ToPlatform());
#endif
            });

        EntryHandler.CommandMapper.AppendToMapping(
            SelectAllCommand,
            static (handler, view, args) =>
            {
                MapperTrace.Record("command mapper: SelectAll");
#if ANDROID
                handler.PlatformView.SetSelection(
                    0,
                    handler.PlatformView.Text?.Length ?? 0);
#endif
            });
    }
}

public sealed class TracingEntryHandler : EntryHandler
{
    private static int _connected;
    private static int _disconnected;

    public static int Connected => Volatile.Read(ref _connected);

    public static int Disconnected => Volatile.Read(ref _disconnected);

#if ANDROID
    protected override void ConnectHandler(MauiAppCompatEditText platformView)
    {
        base.ConnectHandler(platformView);
        platformView.FocusChange += OnFocusChanged;
        Interlocked.Increment(ref _connected);
        MapperTrace.Record($"connect: {platformView.GetType().Name}");
    }

    protected override void DisconnectHandler(MauiAppCompatEditText platformView)
    {
        platformView.FocusChange -= OnFocusChanged;
        Interlocked.Increment(ref _disconnected);
        MapperTrace.Record($"disconnect: {platformView.GetType().Name}");
        base.DisconnectHandler(platformView);
    }

    private static void OnFocusChanged(
        object? sender,
        Android.Views.View.FocusChangeEventArgs e)
    {
        MapperTrace.Record($"native focus={e.HasFocus}");
    }
#endif
}
