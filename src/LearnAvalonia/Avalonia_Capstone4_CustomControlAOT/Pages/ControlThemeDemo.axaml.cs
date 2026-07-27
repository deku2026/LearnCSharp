// Doc:    第10部分-Capstone4-自定义控件渲染与Native-AOT.md §1.4
// Title:  ControlTheme
// Stage:  Capstone4
// Accept: ControlTheme 注册并应用
// Notes:  ControlTheme 在 App.axaml.Styles 内 x:Key=typeof(GaugeControl)。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Capstone4_CustomControlAOT.Pages;

public partial class ControlThemeDemo : UserControl
{
    public ControlThemeDemo()
    {
        InitializeComponent();
    }
}
