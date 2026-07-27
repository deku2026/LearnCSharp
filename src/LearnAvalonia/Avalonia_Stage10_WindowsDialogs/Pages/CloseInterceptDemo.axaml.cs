// Doc:    第5部分-阶段10-桌面窗口对话框与系统集成.md §2.9
// Title:  关闭拦截与未保存
// Stage:  Stage10
// Accept: Closing 异步取消后调用 Close()
// Notes:  Closing 同步事件；e.Cancel=true 再异步询问。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage10_WindowsDialogs.Pages;

public partial class CloseInterceptDemo : UserControl
{
    public CloseInterceptDemo()
    {
        InitializeComponent();
    }
}
