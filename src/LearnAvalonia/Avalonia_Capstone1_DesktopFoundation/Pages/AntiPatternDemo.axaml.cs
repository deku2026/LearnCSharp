// Doc:    第4部分-Capstone1-生产级桌面基础应用.md §8
// Title:  反模式清单
// Stage:  Capstone1
// Accept: 无反模式出现
// Notes:  出现则整改；上帝窗口/静态服务定位器/ViewLocator反射/只读表上DataGrid 等。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Capstone1_DesktopFoundation.Pages;

public partial class AntiPatternDemo : UserControl
{
    public AntiPatternDemo()
    {
        InitializeComponent();
    }
}
