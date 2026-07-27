// Doc:    第2部分-阶段2-Dispatcher线程与UI亲和性.md §7
// Title:  剪贴板与 IAsyncDataTransfer 死锁禁区
// Stage:  Stage02
// Accept: 列出剪贴板回调禁同步 Invoke
// Notes:  展示剪贴板回调同步 Invoke 死锁链与预先快照纪律。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage02_Dispatcher.Pages;

public partial class ClipboardDeadlockDemo : UserControl
{
    public ClipboardDeadlockDemo()
    {
        InitializeComponent();
    }
}
