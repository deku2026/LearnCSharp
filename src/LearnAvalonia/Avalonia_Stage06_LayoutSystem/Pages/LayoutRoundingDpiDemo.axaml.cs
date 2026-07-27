// Doc:    第4部分-阶段6-布局系统.md §B2
// Title:  Layout Rounding 与 DPI/Scaling
// Stage:  Stage06
// Accept: 在分数缩放下验证无 1px 错位灾难
// Notes:  子像素坐标导致模糊边框与缝隙；layout rounding 对齐像素边界。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage06_LayoutSystem.Pages;

public partial class LayoutRoundingDpiDemo : UserControl
{
    public LayoutRoundingDpiDemo()
    {
        InitializeComponent();
        TopLevel? topLevel = TopLevel.GetTopLevel(this);
        double scaling = topLevel?.RenderScaling ?? 1.0;
        ScalingText.Text = $"当前 RenderScaling = {scaling}";
    }
}
