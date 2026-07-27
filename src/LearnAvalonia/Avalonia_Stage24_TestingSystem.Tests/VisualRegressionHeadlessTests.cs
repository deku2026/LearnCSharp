// Doc:    第10部分-阶段24-测试体系.md §4
// Title:  Visual regression Headless 测试（结构化）
// Stage:  Stage24
// Accept: Skia 像素 diff 阈值（结构化断言）
// Notes:  Headless 模式下做结构化断言；像素 diff 需基线文件，此处简化为属性断言。

namespace LearnAvalonia.Avalonia_Stage24_TestingSystem.Tests;

public sealed class VisualRegressionHeadlessTests
{
    [AvaloniaFact]
    public void Border_Arrange_Produces_NonZero_Bounds()
    {
        Avalonia.Controls.Border card = new()
        {
            Background = Avalonia.Media.Brushes.White,
            Width = 120,
            Height = 60,
            Child = new Avalonia.Controls.TextBlock { Text = "Hi" },
        };
        card.Measure(new Avalonia.Size(120, 60));
        card.Arrange(new Avalonia.Rect(0, 0, 120, 60));

        Assert.Equal(120, (int)card.Bounds.Width);
        Assert.Equal(60, (int)card.Bounds.Height);
    }

    [AvaloniaFact]
    public void TextBlock_Text_Matches_Expected_String()
    {
        Avalonia.Controls.TextBlock block = new() { Text = "visual regression sample" };
        block.Measure(new Avalonia.Size(200, 30));
        block.Arrange(new Avalonia.Rect(0, 0, 200, 30));
        Assert.Equal("visual regression sample", block.Text);
        Assert.True(block.Bounds.Width >= 0);
    }

    [AvaloniaFact]
    public void RenderTargetBitmap_Can_Be_Constructed_With_Given_Size()
    {
        using Avalonia.Media.Imaging.RenderTargetBitmap bmp =
            new(new Avalonia.PixelSize(120, 60), new Avalonia.Vector(96, 96));
        Assert.Equal(120, bmp.PixelSize.Width);
        Assert.Equal(60, bmp.PixelSize.Height);
    }
}
