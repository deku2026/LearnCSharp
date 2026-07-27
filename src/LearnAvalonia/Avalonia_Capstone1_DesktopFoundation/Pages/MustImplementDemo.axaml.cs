// Doc:    第4部分-Capstone1-生产级桌面基础应用.md §4
// Title:  必须项详述（XAML/DI/Theme/导航/对话框/Storage）
// Stage:  Capstone1
// Accept: VM 零 Window 类型引用
// Notes:  所有业务 View x:CompileBindings + x:DataType；DataTemplate 必须带 x:DataType。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Capstone1_DesktopFoundation.Pages;

public partial class MustImplementDemo : UserControl
{
    public MustImplementDemo()
    {
        InitializeComponent();
    }
}
