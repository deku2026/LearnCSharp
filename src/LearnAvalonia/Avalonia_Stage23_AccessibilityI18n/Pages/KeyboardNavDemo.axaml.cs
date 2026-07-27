// Doc:    第10部分-阶段23-无障碍国际化与文本.md §2
// Title:  键盘导航与焦点
// Stage:  Stage23
// Accept: Tab/Arrow/Drag 验收可演示
// Notes:  TabStop/TabIndex/KeyboardNavigation；自定义箭头导航；FocusVisible 与 Drag。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage23_AccessibilityI18n.Pages;

public partial class KeyboardNavDemo : UserControl
{
    public KeyboardNavDemo()
    {
        InitializeComponent();
    }

    private void FocusFirstButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (TargetInput is not null)
        {
            TargetInput.Focus();
        }
    }
}
