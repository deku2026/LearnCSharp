// Doc:    第4部分-阶段8-样式主题与ControlTheme.md §D1
// Title:  Static·Dynamic·ThemeResource
// Stage:  Stage08
// Accept: 演示 Static/Dynamic/ThemeResource 差异
// Notes:  真·编译期常量 → StaticResource；运行期可替换 → DynamicResource；随 Light/Dark → ThemeResource。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage08_StylesThemes.Pages;

public partial class ResourceKindsDemo : UserControl
{
    public ResourceKindsDemo()
    {
        InitializeComponent();
    }
}
