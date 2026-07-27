// Doc:    第11部分-阶段31-渲染专家档.md §13
// Title:  GPU 失效恢复
// Stage:  Stage31
// Accept: 列出降级行为
// Notes:  设备丢失 → Renderer 重建资源 → 切软件。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage31_RenderingExpert.Pages;

public partial class GpuLossDemo : UserControl
{
    public GpuLossDemo()
    {
        InitializeComponent();
    }
}
