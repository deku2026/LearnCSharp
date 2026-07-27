// Doc:    第10部分-阶段24-测试体系.md §5
// Title:  Appium 与真窗口
// Stage:  Stage24
// Accept: Appium WinAppDriver 注释示例
// Notes:  Appium 需真窗口；Windows 用 WinAppDriver；跨平台需 platform driver。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage24_TestingSystem.Pages;

public partial class AppiumDemo : UserControl
{
    public AppiumDemo()
    {
        InitializeComponent();
    }
}
