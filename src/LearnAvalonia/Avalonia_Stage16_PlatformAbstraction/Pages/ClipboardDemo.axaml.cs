// Doc:    第7部分-阶段16-统一平台抽象.md §3
// Title:  Clipboard (IAsyncDataTransfer)
// Stage:  Stage16
// Accept: Clipboard.SetTextAsync/TryGetTextAsync
// Notes:  12 起 IAsyncDataTransfer；TopLevel.GetTopLevel(this)?.Clipboard；异步。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage16_PlatformAbstraction.Pages;

public partial class ClipboardDemo : UserControl
{
    public ClipboardDemo()
    {
        InitializeComponent();
    }
}
