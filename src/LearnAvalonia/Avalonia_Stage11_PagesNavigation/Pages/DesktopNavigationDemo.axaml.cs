// Doc:    第5部分-阶段11-Pages与导航架构.md §2.13
// Title:  桌面 document / tab / multi-window
// Stage:  Stage11
// Accept: INavigationService 多实现共享契约
// Notes:  桌面 4 种壳；INavigationService 多实现。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage11_PagesNavigation.Pages;

public partial class DesktopNavigationDemo : UserControl
{
    public DesktopNavigationDemo()
    {
        InitializeComponent();
    }
}
