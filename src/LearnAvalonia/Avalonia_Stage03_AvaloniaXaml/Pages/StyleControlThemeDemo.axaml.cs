// Doc:    第3部分-阶段3-Avalonia-XAML.md §6
// Title:  样式、ControlTheme 与不要用 WPF Trigger 硬套
// Stage:  Stage03
// Accept: 不把 WPF Trigger 硬套 Avalonia
// Notes:  展示 Selector+Setter 样式与 WPF 心智差异。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage03_AvaloniaXaml.Pages;

public partial class StyleControlThemeDemo : UserControl
{
    public StyleControlThemeDemo()
    {
        InitializeComponent();
    }
}
