// Doc:    第5部分-阶段10-桌面窗口对话框与系统集成.md §2.4
// Title:  透明、亚克力与 Topmost
// Stage:  Stage10
// Accept: TransparencyLevelHint 可降级
// Notes:  AcrylicBlur/Blur/Transparent 列表；失败退回实色。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage10_WindowsDialogs.Pages;

public partial class TransparencyTopmostDemo : UserControl
{
    public TransparencyTopmostDemo()
    {
        InitializeComponent();
    }
}
