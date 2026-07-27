// Doc:    第4部分-阶段8-样式主题与ControlTheme.md §C1
// Title:  FluentTheme 与 ThemeVariant
// Stage:  Stage08
// Accept: 切换 ThemeVariant 即时生效
// Notes:  FluentTheme 现代 Fluent 风，模板完整，默认首选；SimpleTheme 更轻更朴素。
using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;

namespace LearnAvalonia.Avalonia_Stage08_StylesThemes.Pages;

public partial class FluentThemeVariantDemo : UserControl
{
    public FluentThemeVariantDemo()
    {
        InitializeComponent();
    }

    private void SetVariant(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (sender is RadioButton rb && rb.Tag is string tag)
        {
            Application.Current!.RequestedThemeVariant = tag switch
            {
                "Light" => ThemeVariant.Light,
                "Dark" => ThemeVariant.Dark,
                _ => ThemeVariant.Default,
            };
        }
    }
}
