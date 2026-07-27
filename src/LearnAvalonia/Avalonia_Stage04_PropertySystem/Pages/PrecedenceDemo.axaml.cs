// Doc:    第3部分-阶段4-属性系统.md §5
// Title:  Value Precedence（值优先级）
// Stage:  Stage04
// Accept: 实验证明覆盖关系
// Notes:  交互式：样式蓝 → SetValue 红（本地压样式） → ClearValue 回蓝。
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace LearnAvalonia.Avalonia_Stage04_PropertySystem.Pages;

public partial class PrecedenceDemo : UserControl
{
    public PrecedenceDemo()
    {
        InitializeComponent();
    }

    private void StyleOnlyButton_Click(object? sender, RoutedEventArgs e)
    {
        if (TargetBorder is not null)
        {
            TargetBorder.ClearValue(Border.BackgroundProperty);
            if (LogText is not null)
            {
                LogText.Text = "日志：ClearValue → 回到样式蓝色（有效值回退到样式 Setter）";
            }
        }
    }

    private void SetValueRedButton_Click(object? sender, RoutedEventArgs e)
    {
        if (TargetBorder is not null)
        {
            TargetBorder.SetValue(Border.BackgroundProperty, Brushes.Crimson);
            if (LogText is not null)
            {
                LogText.Text = "日志：SetValue 红色 → 本地值压过样式（GetValue 返回红）";
            }
        }
    }

    private void ClearValueButton_Click(object? sender, RoutedEventArgs e)
    {
        if (TargetBorder is not null)
        {
            TargetBorder.ClearValue(Border.BackgroundProperty);
            if (LogText is not null)
            {
                LogText.Text = "日志：ClearValue → 本地值清除，有效值回退到样式蓝色";
            }
        }
    }
}
