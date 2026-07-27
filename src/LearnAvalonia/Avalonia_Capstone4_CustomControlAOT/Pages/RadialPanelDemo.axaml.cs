// Doc:    第10部分-Capstone4-自定义控件渲染与Native-AOT.md §1.5
// Title:  RadialPanel 自定义布局
// Stage:  Capstone4
// Accept: MeasureOverride + ArrangeOverride
// Notes:  下方 RadialPanel 把 6 个 Border 均匀分布在圆周上。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Capstone4_CustomControlAOT.Pages;

public partial class RadialPanelDemo : UserControl
{
    public RadialPanelDemo()
    {
        InitializeComponent();
    }
}
