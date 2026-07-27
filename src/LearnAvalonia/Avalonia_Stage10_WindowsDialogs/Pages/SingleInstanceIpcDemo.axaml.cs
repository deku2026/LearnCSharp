// Doc:    第5部分-阶段10-桌面窗口对话框与系统集成.md §2.10
// Title:  单实例与 IPC 激活
// Stage:  Stage10
// Accept: Mutex + 命名管道激活已运行实例
// Notes:  Mutex 防多实例；第二实例经 IPC 唤起第一实例。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage10_WindowsDialogs.Pages;

public partial class SingleInstanceIpcDemo : UserControl
{
    public SingleInstanceIpcDemo()
    {
        InitializeComponent();
    }
}
