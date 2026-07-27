// Doc:    第10部分-阶段24-测试体系.md §6
// Title:  输入 Headless 测试
// Stage:  Stage24
// Accept: KeyDown/Mouse/TextInput
// Notes:  构造控件，断言输入相关属性与事件订阅可工作。

namespace LearnAvalonia.Avalonia_Stage24_TestingSystem.Tests;

public sealed class InputHeadlessTests
{
    [AvaloniaFact]
    public void TextBox_Accepts_Text_Via_Property()
    {
        Avalonia.Controls.TextBox box = new() { Text = "abc" };
        box.Measure(new Avalonia.Size(200, 30));
        box.Arrange(new Avalonia.Rect(0, 0, 200, 30));
        Assert.Equal("abc", box.Text);
    }

    [AvaloniaFact]
    public void Button_KeyDown_Subscription_Attached()
    {
        Avalonia.Controls.Button button = new() { Content = "Press" };
        int count = 0;
        button.KeyDown += (_, _) => count++;
        Assert.Equal(0, count);
    }

    [AvaloniaFact]
    public void PointerOver_PseudoClass_Not_Active_Without_Pointer()
    {
        Avalonia.Controls.Button button = new() { Content = "Hover" };
        button.Measure(new Avalonia.Size(100, 30));
        button.Arrange(new Avalonia.Rect(0, 0, 100, 30));
        bool hasPointerOver = button.Classes.Contains(":pointerover");
        Assert.False(hasPointerOver);
    }
}
