// Doc:    第10部分-阶段23-无障碍国际化与文本.md §4
// Title:  语义、高对比、减弱动效、动态字号
// Stage:  Stage23
// Accept: 切换 ThemeVariant 与动效开关
// Notes:  Application.RequestedThemeVariant；HighContrast 需自定义主题；减弱动效靠禁用动画；字号 FontSizing。
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Styling;

namespace LearnAvalonia.Avalonia_Stage23_AccessibilityI18n.Pages;

public partial class HighContrastDemo : UserControl
{
    private static readonly ThemeVariant s_highContrast = new("HighContrast", null);

    public HighContrastDemo()
    {
        InitializeComponent();
    }

    private void ToggleHighContrast_Click(object? sender, RoutedEventArgs e)
    {
        if (Application.Current is not null)
        {
            ThemeVariant current = Application.Current.RequestedThemeVariant ?? ThemeVariant.Default;
            Application.Current.RequestedThemeVariant = current == s_highContrast
                ? ThemeVariant.Default
                : s_highContrast;
        }

        if (StatusText is not null)
        {
            string variant = Application.Current?.RequestedThemeVariant?.ToString() ?? "?";
            StatusText.Text = "当前 ThemeVariant=" + variant + "（注：HighContrast 需主题侧提供资源）";
        }
    }

    private void ToggleAnimations_Click(object? sender, RoutedEventArgs e)
    {
        if (AnimationTarget is not null)
        {
            AnimationTarget.IsVisible = !AnimationTarget.IsVisible;
        }
    }
}
