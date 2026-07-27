// Doc:    第10部分-阶段24-测试体系.md §6
// Title:  控件 Headless 测试
// Stage:  Stage24
// Accept: Click/Selection/IsEnabled
// Notes:  构造 Button/ListBox，模拟事件，断言状态变化。

namespace LearnAvalonia.Avalonia_Stage24_TestingSystem.Tests;

public sealed class ControlHeadlessTests
{
    [AvaloniaFact]
    public void Button_Click_Raises_RoutedEvent()
    {
        Avalonia.Controls.Button button = new() { Content = "Click" };
        button.Measure(new Avalonia.Size(100, 30));
        button.Arrange(new Avalonia.Rect(0, 0, 100, 30));

        bool clicked = false;
        button.Click += (_, _) => clicked = true;
        button.RaiseEvent(new Avalonia.Interactivity.RoutedEventArgs(Avalonia.Controls.Button.ClickEvent));

        Assert.True(clicked);
    }

    [AvaloniaFact]
    public void Button_IsEnabled_False_Reports_Disabled_State()
    {
        Avalonia.Controls.Button button = new() { Content = "Click", IsEnabled = false };
        button.Measure(new Avalonia.Size(100, 30));
        button.Arrange(new Avalonia.Rect(0, 0, 100, 30));

        Assert.False(button.IsEnabled);
    }

    [AvaloniaFact]
    public void ListBox_Selection_Single_Updates_SelectedItem()
    {
        System.Collections.ObjectModel.ObservableCollection<string> items = ["A", "B", "C"];
        Avalonia.Controls.ListBox list = new() { ItemsSource = items };
        list.Measure(new Avalonia.Size(200, 200));
        list.Arrange(new Avalonia.Rect(0, 0, 200, 200));

        list.SelectedIndex = 1;
        Assert.Equal("B", list.SelectedItem);
        Assert.Equal(1, list.SelectedIndex);
    }
}
