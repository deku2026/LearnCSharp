// Doc:    第9部分-Capstone2-复杂桌面生产应用.md §12
// Title:  UI 自动化与测试
// Stage:  Capstone2
// Accept: 单元 + Headless 冒烟绿
// Notes:  Undo栈/同步冲突/设置迁移/VM命令 单测；Avalonia.Headless.XUnit 12.1.0 冒烟。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Capstone2_ComplexDesktop.Pages;

public partial class UiAutomationTestDemo : UserControl
{
    public UiAutomationTestDemo()
    {
        InitializeComponent();
    }
}
