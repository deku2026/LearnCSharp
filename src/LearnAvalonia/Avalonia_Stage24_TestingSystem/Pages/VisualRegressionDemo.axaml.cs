// Doc:    第10部分-阶段24-测试体系.md §4
// Title:  Visual regression（Headless + Skia 像素）
// Stage:  Stage24
// Accept: Skia 像素 diff 阈值
// Notes:  渲染控件到 Skia bitmap；与基线 diff；阈值容差。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage24_TestingSystem.Pages;

public partial class VisualRegressionDemo : UserControl
{
    public VisualRegressionDemo()
    {
        InitializeComponent();
    }
}
