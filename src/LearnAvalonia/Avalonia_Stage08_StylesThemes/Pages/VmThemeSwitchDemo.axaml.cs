// Doc:    第4部分-阶段8-样式主题与ControlTheme.md §F4
// Title:  VM 切换主题与跨平台抽检
// Stage:  Stage08
// Accept: VM 切换 ThemeVariant 即时生效
// Notes:  Application.Current.RequestedThemeVariant = value switch。
using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;

namespace LearnAvalonia.Avalonia_Stage08_StylesThemes.Pages;

public partial class VmThemeSwitchDemo : UserControl
{
    public VmThemeSwitchDemo()
    {
        InitializeComponent();
    }

    private void OnThemeChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox cb && cb.SelectedIndex is int idx)
        {
            Application.Current!.RequestedThemeVariant = idx switch
            {
                1 => ThemeVariant.Light,
                2 => ThemeVariant.Dark,
                _ => ThemeVariant.Default,
            };
        }
    }
}
