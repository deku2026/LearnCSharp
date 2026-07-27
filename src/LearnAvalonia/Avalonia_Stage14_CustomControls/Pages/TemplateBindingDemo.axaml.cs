// Doc:    第6部分-阶段14-自定义控件.md §2.5
// Title:  TemplateBinding vs Binding
// Stage:  Stage14
// Accept: 模板内必须 TemplateBinding 控件属性
// Notes:  TemplateBinding 绑模板父；{Binding} 在模板内是 DataContext。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage14_CustomControls.Pages;

public partial class TemplateBindingDemo : UserControl
{
    public TemplateBindingDemo()
    {
        InitializeComponent();
    }
}
