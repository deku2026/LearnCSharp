// Doc:    第10部分-Capstone4-自定义控件渲染与Native-AOT.md §1.8
// Title:  无障碍 AutomationPeer
// Stage:  Capstone4
// Accept: OnCreateAutomationPeer 返回
// Notes:  GaugeAutomationPeer 实现 IRangeValueProvider；读屏识别 Value/Maximum。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Capstone4_CustomControlAOT.Pages;

public partial class AccessibilityDemo : UserControl
{
    public AccessibilityDemo()
    {
        InitializeComponent();
    }
}
