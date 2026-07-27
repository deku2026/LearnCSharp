// Doc:    第11部分-阶段31-渲染专家档.md §5
// Title:  GPU 上下文
// Stage:  Stage31
// Accept: 列出失败模式与切换
// Notes:  IPlatformGraphics 提供 IPlatformGraphicsContext；GPU 失败时切软件。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage31_RenderingExpert.Pages;

public partial class GpuContextDemo : UserControl
{
    public GpuContextDemo()
    {
        InitializeComponent();
    }
}
