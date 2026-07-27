// Doc:    第3部分-阶段3-Avalonia-XAML.md §9
// Title:  同一 UI：axaml 与纯 C# 对照实验
// Stage:  Stage03
// Accept: 完成 PersonCard 双实现
// Notes:  展示 axaml 与纯 C# 构造等价对象图；CsCardHost 注入 C# 构造的卡片。
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace LearnAvalonia.Avalonia_Stage03_AvaloniaXaml.Pages;

public partial class AxamlVsCsDemo : UserControl
{
    public AxamlVsCsDemo()
    {
        InitializeComponent();
        if (CsCardHost is not null)
        {
            TextBlock name = new() { FontWeight = FontWeight.Bold, Text = "张三（C# 构造）" };
            TextBlock email = new() { Opacity = 0.7, Text = "zhangsan@example.com" };
            Button edit = new() { Content = "编辑", Margin = new Thickness(0, 4, 0, 0) };
            CsCardHost.Content = new Border
            {
                Padding = new Thickness(12),
                CornerRadius = new CornerRadius(8),
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(1),
                Child = new StackPanel
                {
                    Spacing = 4,
                    Children = { name, email, edit },
                },
            };
        }
    }
}
