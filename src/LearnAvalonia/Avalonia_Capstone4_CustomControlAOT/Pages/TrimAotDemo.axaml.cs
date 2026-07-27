// Doc:    第10部分-Capstone4-自定义控件渲染与Native-AOT.md §5
// Title:  Trim / Native AOT
// Stage:  Capstone4
// Accept: IsAotCompatible + 警告
// Notes:  csproj 设 <IsAotCompatible>true</IsAotCompatible>；反射慎用。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Capstone4_CustomControlAOT.Pages;

public partial class TrimAotDemo : UserControl
{
    public TrimAotDemo()
    {
        InitializeComponent();
    }
}
