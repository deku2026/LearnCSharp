// Doc:    第2部分-阶段1-AppBuilder与应用启动.md §4
// Title:  IApplicationLifetime 族详解
// Stage:  Stage01
// Accept: 区分 Desktop/SingleView/Activity
// Notes:  展示接口层次与三种 lifetime 的宿主对照。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage01_AppBuilder.Pages;

public partial class ApplicationLifetimeDemo : UserControl
{
    public ApplicationLifetimeDemo()
    {
        InitializeComponent();
    }
}
