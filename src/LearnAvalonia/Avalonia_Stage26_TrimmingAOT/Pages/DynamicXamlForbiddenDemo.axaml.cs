// Doc:    第10部分-阶段26-Trimming与Native-AOT.md §3
// Title:  动态 XAML 禁区
// Stage:  Stage26
// Accept: 运行时 XAML 不兼容 AOT
// Notes:  AvaloniaXamlLoader.Load 动态加载需保留；AOT 下禁用或 IsAotCompatible=false。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage26_TrimmingAOT.Pages;

public partial class DynamicXamlForbiddenDemo : UserControl
{
    public DynamicXamlForbiddenDemo()
    {
        InitializeComponent();
    }
}
