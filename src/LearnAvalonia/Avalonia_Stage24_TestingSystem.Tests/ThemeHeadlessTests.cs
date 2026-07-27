// Doc:    第10部分-阶段24-测试体系.md §6
// Title:  主题 Headless 测试
// Stage:  Stage24
// Accept: Light/Dark 切换资源
// Notes:  切换 Application.RequestedThemeVariant，断言资源解析。

namespace LearnAvalonia.Avalonia_Stage24_TestingSystem.Tests;

public sealed class ThemeHeadlessTests
{
    [AvaloniaFact]
    public void RequestedThemeVariant_Can_Be_Switched_To_Dark()
    {
        Avalonia.Application? app = Avalonia.Application.Current;
        Assert.NotNull(app);
        app.RequestedThemeVariant = Avalonia.Styling.ThemeVariant.Dark;
        Assert.Equal(Avalonia.Styling.ThemeVariant.Dark, app.RequestedThemeVariant);
        app.RequestedThemeVariant = Avalonia.Styling.ThemeVariant.Default;
    }

    [AvaloniaFact]
    public void ThemeVariant_Light_And_Dark_Are_Distinct()
    {
        Assert.NotEqual(Avalonia.Styling.ThemeVariant.Light, Avalonia.Styling.ThemeVariant.Dark);
        Assert.NotEqual(Avalonia.Styling.ThemeVariant.Default, Avalonia.Styling.ThemeVariant.Dark);
    }
}
