// Doc:    第7部分-阶段16-统一平台抽象.md §4
// Title:  Launcher（URI/外部）
// Stage:  Stage16
// Accept: Launcher.LaunchUriAsync
// Notes:  Launcher.LaunchUriAsync(Uri)；桌面打开浏览器/邮件；返回 bool。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage16_PlatformAbstraction.Pages;

public partial class LauncherDemo : UserControl
{
    public LauncherDemo()
    {
        InitializeComponent();
    }
}
