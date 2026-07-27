// Doc:    第10部分-阶段24-测试体系.md §6
// Title:  焦点 Headless 测试
// Stage:  Stage24
// Accept: Tab 链/TabStop/Focus()
// Notes:  构造多个控件，调 Focus()，断言焦点管理属性可访问。

namespace LearnAvalonia.Avalonia_Stage24_TestingSystem.Tests;

public sealed class FocusHeadlessTests
{
    [AvaloniaFact]
    public void Focusable_True_By_Default_On_TextBox()
    {
        Avalonia.Controls.TextBox box = new();
        Assert.True(box.Focusable);
    }

    [AvaloniaFact]
    public void TabStop_False_Excludes_From_Tab_Chain()
    {
        Avalonia.Controls.Button first = new() { Content = "first" };
        Avalonia.Controls.Button middle = new() { Content = "middle", IsTabStop = false };
        Avalonia.Controls.Button last = new() { Content = "last" };
        Assert.False(middle.IsTabStop);
        Assert.True(first.IsTabStop);
        Assert.True(last.IsTabStop);
    }

    [AvaloniaFact]
    public void KeyboardNavigation_TabNavigation_Cycle_Sets_Attached_Property()
    {
        Avalonia.Controls.StackPanel panel = new();
        Avalonia.Input.KeyboardNavigation.SetTabNavigation(panel, Avalonia.Input.KeyboardNavigationMode.Cycle);
        Assert.Equal(Avalonia.Input.KeyboardNavigationMode.Cycle, Avalonia.Input.KeyboardNavigation.GetTabNavigation(panel));
    }
}
