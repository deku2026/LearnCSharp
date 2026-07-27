// Doc:    第4部分-阶段8-样式主题与ControlTheme.md §E1
// Title:  ControlTheme 与 BasedOn
// Stage:  Stage08
// Accept: 使用 Theme 属性指定 ControlTheme
// Notes:  ControlTheme 是某控件类型的主题包；常以类型为 key；可 BasedOn 继承。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage08_StylesThemes.Pages;

public partial class ControlThemeBasedOnDemo : UserControl
{
    public ControlThemeBasedOnDemo()
    {
        InitializeComponent();
    }
}
