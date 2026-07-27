// Doc:    第5部分-阶段12-输入焦点手势与IME.md §2.9
// Title:  剪贴板（IAsyncDataTransfer）
// Stage:  Stage12
// Accept: TryGetTextAsync 取代 GetTextAsync
// Notes:  TopLevel.Clipboard 异步；SetTextAsync/TryGetTextAsync。
using Avalonia.Controls;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;

namespace LearnAvalonia.Avalonia_Stage12_InputFocusGestures.Pages;

public partial class ClipboardDemo : UserControl
{
    public ClipboardDemo()
    {
        InitializeComponent();
    }

    private async void CopyBtn_Clicked(object? sender, RoutedEventArgs e)
    {
        IClipboard? clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
        if (clipboard is null) { Result.Text = "剪贴板不可用"; return; }
        await clipboard.SetTextAsync("复制自 Avalonia 12");
        Result.Text = "已复制：复制自 Avalonia 12";
    }

    private async void PasteBtn_Clicked(object? sender, RoutedEventArgs e)
    {
        IClipboard? clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
        if (clipboard is null) { Result.Text = "剪贴板不可用"; return; }
        string? text = await clipboard.TryGetTextAsync();
        Result.Text = text is null ? "(剪贴板无文本)" : text;
    }
}
