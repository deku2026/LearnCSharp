// Doc:    第5部分-阶段10-桌面窗口对话框与系统集成.md §2.3
// Title:  WindowDecorations 与自绘标题栏
// Stage:  Stage10
// Accept: SystemDecorations 已改名 WindowDecorations
// Notes:  12 主线：SystemDecorations → WindowDecorations；Chrome 走 WindowDrawnDecorations。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage10_WindowsDialogs.Pages;

public partial class WindowDecorationsDemo : UserControl
{
    public WindowDecorationsDemo()
    {
        InitializeComponent();
    }
}
