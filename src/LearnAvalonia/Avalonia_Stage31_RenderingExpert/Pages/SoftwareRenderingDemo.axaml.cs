// Doc:    第11部分-阶段31-渲染专家档.md §10
// Title:  软件渲染
// Stage:  Stage31
// Accept: 解释降级与取舍
// Notes:  GPU 不可用时切 Skia raster；性能差但保证可见。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage31_RenderingExpert.Pages;

public partial class SoftwareRenderingDemo : UserControl
{
    public SoftwareRenderingDemo()
    {
        InitializeComponent();
    }
}
