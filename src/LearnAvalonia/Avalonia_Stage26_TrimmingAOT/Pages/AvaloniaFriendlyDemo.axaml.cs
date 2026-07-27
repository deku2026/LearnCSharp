// Doc:    第10部分-阶段26-Trimming与Native-AOT.md §2
// Title:  Avalonia 特有友好点
// Stage:  Stage26
// Accept: CompiledBinding 友好 AOT
// Notes:  CompiledBinding 编译期；Avalonia 12 已 AOT 兼容；markup 编译。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage26_TrimmingAOT.Pages;

public partial class AvaloniaFriendlyDemo : UserControl
{
    public AvaloniaFriendlyDemo()
    {
        InitializeComponent();
    }
}
