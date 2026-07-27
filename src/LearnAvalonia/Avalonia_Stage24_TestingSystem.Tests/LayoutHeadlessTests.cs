// Doc:    第10部分-阶段24-测试体系.md §6
// Title:  布局 Headless 测试
// Stage:  Stage24
// Accept: Measure/Arrange/DesiredSize
// Notes:  构造 StackPanel + 多个子控件，触发 Measure/Arrange，断言 DesiredSize 和 Bounds。

namespace LearnAvalonia.Avalonia_Stage24_TestingSystem.Tests;

public sealed class LayoutHeadlessTests
{
    [AvaloniaFact]
    public void StackPanel_Measures_Children_And_Arranges_Vertically()
    {
        Avalonia.Controls.StackPanel panel = new()
        {
            Orientation = Avalonia.Layout.Orientation.Vertical,
            Spacing = 4,
        };
        Avalonia.Controls.TextBlock first = new() { Text = "first", FontSize = 14 };
        Avalonia.Controls.TextBlock second = new() { Text = "second", FontSize = 14 };
        panel.Children.Add(first);
        panel.Children.Add(second);

        panel.Measure(new Avalonia.Size(200, 400));
        panel.Arrange(new Avalonia.Rect(0, 0, 200, panel.DesiredSize.Height));

        Assert.True(panel.DesiredSize.Height > 0);
        Assert.True(first.Bounds.Top < second.Bounds.Top);
        Assert.True(second.Bounds.Top - first.Bounds.Top >= first.Bounds.Height);
    }

    [AvaloniaFact]
    public void Grid_Arranges_Two_Columns_By_Star_Definitions()
    {
        Avalonia.Controls.Grid grid = new()
        {
            ColumnDefinitions = new Avalonia.Controls.ColumnDefinitions("*,*"),
        };
        Avalonia.Controls.Border left = new() { Background = Avalonia.Media.Brushes.Red, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch };
        Avalonia.Controls.Border right = new() { Background = Avalonia.Media.Brushes.Blue, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch };
        Avalonia.Controls.Grid.SetColumn(left, 0);
        Avalonia.Controls.Grid.SetColumn(right, 1);
        grid.Children.Add(left);
        grid.Children.Add(right);

        grid.Measure(new Avalonia.Size(200, 50));
        grid.Arrange(new Avalonia.Rect(0, 0, 200, 50));

        Assert.True(left.Bounds.Width > 0);
        Assert.True(right.Bounds.Width > 0);
        Assert.True((int)(left.Bounds.Width + right.Bounds.Width) >= 100);
    }
}
