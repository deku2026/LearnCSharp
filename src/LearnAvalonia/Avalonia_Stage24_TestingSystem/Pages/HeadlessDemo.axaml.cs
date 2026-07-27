// Doc:    第10部分-阶段24-测试体系.md §3
// Title:  Headless 测试完整示例
// Stage:  Stage24
// Accept: AvaloniaFact 构造控件 + 断言
// Notes:  Avalonia.Headless.XUnit；[AvaloniaFact] 启内存 Avalonia 应用；Measure/Arrange。
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace LearnAvalonia.Avalonia_Stage24_TestingSystem.Pages;

public partial class HeadlessDemo : UserControl
{
    public HeadlessDemo()
    {
        InitializeComponent();
    }

    private void RunSnippetButton_Click(object? sender, RoutedEventArgs e)
    {
        if (SnippetResult is null)
        {
            return;
        }

        Button target = new() { Content = "Click me" };
        target.Measure(new Avalonia.Size(200, 100));
        target.Arrange(new Avalonia.Rect(0, 0, target.DesiredSize.Width, target.DesiredSize.Height));
        bool clicked = false;
        target.Click += (_, _) => clicked = true;
        target.RaiseEvent(new Avalonia.Interactivity.RoutedEventArgs(Button.ClickEvent));
        SnippetResult.Text = "构造 Button + Measure/Arrange + 模拟 Click 事件\n→ DesiredSize=" + target.DesiredSize + "\n→ Click 触发：" + (clicked ? "是" : "否");
    }
}
