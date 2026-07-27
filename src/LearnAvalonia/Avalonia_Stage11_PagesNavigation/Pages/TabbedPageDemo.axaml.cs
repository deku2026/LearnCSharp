// Doc:    第5部分-阶段11-Pages与导航架构.md §2.5
// Title:  TabbedPage
// Stage:  Stage11
// Accept: Tab 根 + 每 Tab 内嵌 NavigationPage
// Notes:  Tab 根；每 Tab 内嵌 NavigationPage 保持独立返回栈。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage11_PagesNavigation.Pages;

public partial class TabbedPageDemo : UserControl
{
    public TabbedPageDemo()
    {
        InitializeComponent();
    }
}
