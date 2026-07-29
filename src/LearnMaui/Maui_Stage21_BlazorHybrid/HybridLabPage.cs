using System.Text.Json.Serialization;

namespace LearnMaui.Maui_Stage21_BlazorHybrid;

public sealed class HybridLabPage : ContentPage
{
    private readonly HybridWebView _hybrid = new()
    {
        AutomationId = "HybridLab.WebView",
        DefaultFile = "index.html",
        HybridRoot = "wwwroot",
    };

    private readonly Label _status = new()
    {
        Text = "本地可信资源待加载",
        Margin = new Thickness(12, 4),
    };

    public HybridLabPage(HybridBridge bridge)
    {
        Title = "HybridWebView 实验";
        _hybrid.SetInvokeJavaScriptTarget(bridge);
        _hybrid.RawMessageReceived += OnRawMessageReceived;
        _hybrid.HandlerChanged += OnHandlerChanged;
        _hybrid.WebViewInitializing += OnWebViewInitializing;
        _hybrid.WebViewInitialized += OnWebViewInitialized;
        _hybrid.WebResourceRequested += OnWebResourceRequested;

        Button invokeButton = new()
        {
            Text = ".NET → JavaScript",
            AutomationId = "HybridLab.InvokeJavaScript",
        };
        invokeButton.Clicked += OnInvokeJavaScript;

        Content = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Star),
            },
            Children =
            {
                invokeButton,
                _status,
                _hybrid,
            },
        };
        Grid.SetRow(_status, 1);
        Grid.SetRow(_hybrid, 2);
    }

    private async void OnInvokeJavaScript(object? sender, EventArgs e)
    {
        try
        {
            string? result = await _hybrid.InvokeJavaScriptAsync<string>(
                "updateFromDotNet",
                HybridJsonContext.Default.String,
                [$"MAUI @ {DateTimeOffset.Now:HH:mm:ss}"],
                [HybridJsonContext.Default.String]);
            _status.Text = $"JS 返回：{result}";
        }
        catch (Exception exception)
        {
            _status.Text = $"JS 异常已回抛：{exception.Message}";
        }
    }

    private void OnRawMessageReceived(
        object? sender,
        HybridWebViewRawMessageReceivedEventArgs e)
    {
        string message = e.Message ?? string.Empty;
        _status.Text = $"Raw：{message[..Math.Min(message.Length, 200)]}";
    }

    private void OnHandlerChanged(object? sender, EventArgs e)
    {
#if ANDROID
        if (_hybrid.Handler?.PlatformView is Android.Webkit.WebView webView)
        {
            webView.Settings.BuiltInZoomControls = false;
            webView.Settings.DisplayZoomControls = false;
            webView.Settings.AllowFileAccess = false;
        }
#endif
    }

    private void OnWebViewInitializing(
        object? sender,
        WebViewInitializingEventArgs e)
    {
        _status.Text = $"初始化中：{DeviceInfo.Current.Platform}";
    }

    private void OnWebViewInitialized(
        object? sender,
        WebViewInitializedEventArgs e)
    {
        _status.Text = $"已初始化：{_hybrid.Handler?.PlatformView?.GetType().Name ?? "unknown"}";
    }

    private void OnWebResourceRequested(
        object? sender,
        WebViewWebResourceRequestedEventArgs e)
    {
        if (!e.Uri.AbsolutePath.EndsWith("/health.json", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        e.Handled = true;
        e.SetResponse(
            200,
            "OK",
            "application/json",
            Task.FromResult<Stream?>(
                new MemoryStream("""{"status":"offline-ready"}"""u8.ToArray())));
    }

    protected override void OnDisappearing()
    {
        _hybrid.RawMessageReceived -= OnRawMessageReceived;
        _hybrid.HandlerChanged -= OnHandlerChanged;
        _hybrid.WebViewInitializing -= OnWebViewInitializing;
        _hybrid.WebViewInitialized -= OnWebViewInitialized;
        _hybrid.WebResourceRequested -= OnWebResourceRequested;
        base.OnDisappearing();
    }
}

[JsonSourceGenerationOptions(WriteIndented = false)]
[JsonSerializable(typeof(string))]
internal partial class HybridJsonContext : JsonSerializerContext;
