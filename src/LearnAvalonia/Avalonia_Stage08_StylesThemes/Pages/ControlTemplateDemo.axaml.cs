// Doc:    第4部分-阶段8-样式主题与ControlTheme.md §E3
// Title:  ControlTemplate 与无外观控件
// Stage:  Stage08
// Accept: 提供可替换 ControlTheme
// Notes:  TemplateBinding 绑定到模板父控件属性；PART_* 约定代码 OnApplyTemplate 找零件。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage08_StylesThemes.Pages;

public partial class ControlTemplateDemo : UserControl
{
    public ControlTemplateDemo()
    {
        InitializeComponent();
    }
}
