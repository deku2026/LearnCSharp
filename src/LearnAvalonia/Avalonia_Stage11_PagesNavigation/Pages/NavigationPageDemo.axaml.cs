// Doc:    第5部分-阶段11-Pages与导航架构.md §2.3
// Title:  NavigationPage 与 INavigation
// Stage:  Stage11
// Accept: PushAsync/PopAsync/ModalStack 心智
// Notes:  INavigation 异步 API；INavigationService 抽象注入 VM。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage11_PagesNavigation.Pages;

public partial class NavigationPageDemo : UserControl
{
    public NavigationPageDemo()
    {
        InitializeComponent();
    }
}
