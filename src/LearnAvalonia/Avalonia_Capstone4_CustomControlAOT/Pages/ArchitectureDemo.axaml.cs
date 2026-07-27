// Doc:    第10部分-Capstone4-自定义控件渲染与Native-AOT.md §1
// Title:  架构与示例题
// Stage:  Capstone4
// Accept: 列出 GaugeControl + RadialPanel + 仪表表
// Notes:  整体架构：GaugeControl (TemplatedControl) + RadialPanel (Panel) + 仪表表 (demo/学习用)。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Capstone4_CustomControlAOT.Pages;

public partial class ArchitectureDemo : UserControl
{
    public ArchitectureDemo()
    {
        InitializeComponent();
    }
}
