// Doc:    第10部分-阶段24-测试体系.md §2
// Title:  Smoke test
// Stage:  Stage24
// Accept: 测试项目可在 CI 中执行并通过
// Notes:  验证 Headless 测试宿主能加载 Avalonia 应用并运行一个最小断言。

namespace LearnAvalonia.Avalonia_Stage24_TestingSystem.Tests;

public sealed class SmokeTests
{
    [AvaloniaFact]
    public void Headless_host_builds_a_window()
    {
        Avalonia.Controls.Window window = new()
        {
            Width = 200,
            Height = 100,
            Content = new Avalonia.Controls.TextBlock { Text = "headless ok" },
        };
        window.Show();
        Assert.NotNull(window.Content);
    }
}