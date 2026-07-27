// Doc:    第10部分-Capstone4-自定义控件渲染与Native-AOT.md §3
// Title:  测试结构
// Stage:  Capstone4
// Accept: Unit / Headless / 集成 / a11y
// Notes:  单测测 property + 事件；Headless 测 Render 截图；a11y 测 AutomationPeer。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Capstone4_CustomControlAOT.Pages;

public partial class TestStructureDemo : UserControl
{
    public TestStructureDemo()
    {
        InitializeComponent();
    }
}
