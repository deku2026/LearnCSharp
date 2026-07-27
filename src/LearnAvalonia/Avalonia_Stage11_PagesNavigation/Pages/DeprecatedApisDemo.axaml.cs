// Doc:    第5部分-阶段11-Pages与导航架构.md §5
// Title:  过时与禁止
// Stage:  Stage11
// Accept: 禁默认反射 ViewLocator 与同步阻塞
// Notes:  反射 ViewLocator 当生产路由禁止；用显式 IViewFactory。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage11_PagesNavigation.Pages;

public partial class DeprecatedApisDemo : UserControl
{
    public DeprecatedApisDemo()
    {
        InitializeComponent();
    }
}
