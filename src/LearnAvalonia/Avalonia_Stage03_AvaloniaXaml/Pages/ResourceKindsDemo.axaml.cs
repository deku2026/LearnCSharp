// Doc:    第3部分-阶段3-Avalonia-XAML.md §5
// Title:  资源：Static / Dynamic / Theme
// Stage:  Stage03
// Accept: 区分 StaticResource/DynamicResource/ThemeResource
// Notes:  交互式：切换资源键观察 Static 不更新 vs Dynamic 更新。
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace LearnAvalonia.Avalonia_Stage03_AvaloniaXaml.Pages;

public partial class ResourceKindsDemo : UserControl
{
    private bool _toggle;

    public ResourceKindsDemo()
    {
        InitializeComponent();
    }

    private void ToggleBgButton_Click(object? sender, RoutedEventArgs e)
    {
        _toggle = !_toggle;
        Color newColor = _toggle ? Color.FromRgb(0xD4, 0xE6, 0xF9) : Color.FromRgb(0xF3, 0xF3, 0xF3);
        if (Resources is not null)
        {
            Resources["CardBg"] = new SolidColorBrush(newColor);
        }
    }
}
