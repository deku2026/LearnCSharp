// Doc:    第10部分-Capstone4-自定义控件渲染与Native-AOT.md §1.6
// Title:  自定义 Render (DrawingContext)
// Stage:  Capstone4
// Accept: Render 自绘圆弧刻度
// Notes:  GaugeControl.Render 画圆弧 + 刻度 + 数字；下方面板展示关键 Draw 调用。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Capstone4_CustomControlAOT.Pages;

public partial class CustomRenderDemo : UserControl
{
    public CustomRenderDemo()
    {
        InitializeComponent();
    }
}
