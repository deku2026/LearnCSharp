// Doc:    第10部分-阶段28-WPF-WinUI-WinForms迁移与互操作.md §2
// Title:  XAML/样式/模板/绑定/路由事件差异
// Stage:  Stage28
// Accept: XAML 差异并排示例
// Notes:  Trigger 不存在；CompiledBinding 默认；SelectionChanged 不冒泡。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage28_WpfMigration.Pages;

public partial class XamlStyleBindingDemo : UserControl
{
    public XamlStyleBindingDemo()
    {
        InitializeComponent();
    }
}
