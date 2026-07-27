// Doc:    第10部分-Capstone4-自定义控件渲染与Native-AOT.md §1.9
// Title:  本地化
// Stage:  Capstone4
// Accept: ResourceProvider + 多语言
// Notes:  本 demo 演示 .resx 资源 vs 代码切换；运行时切 Culture。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Capstone4_CustomControlAOT.Pages;

public partial class LocalizationDemo : UserControl
{
    public LocalizationDemo()
    {
        InitializeComponent();
    }
}
