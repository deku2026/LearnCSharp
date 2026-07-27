// Doc:    第6部分-阶段15-虚拟化与大数据UI.md §2.1
// Title:  虚拟化原理与容器
// Stage:  Stage15
// Accept: 画出虚拟化容器/recycle 原理
// Notes:  ItemContainer 生成 → 测量 → 布局；视口外容器进入 recycle 池。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage15_Virtualization.Pages;

public partial class VirtualizationPrincipleDemo : UserControl
{
    public VirtualizationPrincipleDemo()
    {
        InitializeComponent();
    }
}
