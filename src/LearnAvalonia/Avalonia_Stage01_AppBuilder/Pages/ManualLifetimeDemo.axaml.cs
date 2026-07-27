// Doc:    第2部分-阶段1-AppBuilder与应用启动.md §6
// Title:  手动 lifetime：Start + app.Run
// Stage:  Stage01
// Accept: 写通过 Start+Run 的桌面最小程序
// Notes:  展示手动主循环写法与可移植性注意点。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage01_AppBuilder.Pages;

public partial class ManualLifetimeDemo : UserControl
{
    public ManualLifetimeDemo()
    {
        InitializeComponent();
    }
}
