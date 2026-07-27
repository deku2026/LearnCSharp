// Doc:    第6部分-阶段15-虚拟化与大数据UI.md §1
// Title:  为什么虚拟化是分水岭
// Stage:  Stage15
// Accept: 解释虚拟化与非虚拟化的临界点
// Notes:  10k 行非虚拟化即卡；虚拟化只实例化可见 ~20 项。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage15_Virtualization.Pages;

public partial class WhyVirtualizationDemo : UserControl
{
    public WhyVirtualizationDemo()
    {
        InitializeComponent();
    }
}
