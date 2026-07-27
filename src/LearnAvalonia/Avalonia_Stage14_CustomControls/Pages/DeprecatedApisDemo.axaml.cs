// Doc:    第6部分-阶段14-自定义控件.md §6
// Title:  过时与禁止
// Stage:  Stage14
// Accept: 禁默认反射 ViewLocator/旧 Trigger
// Notes:  WPF OverrideMetadata 照搬禁；TemplateBinding；Measure 改属性死循环。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage14_CustomControls.Pages;

public partial class DeprecatedApisDemo : UserControl
{
    public DeprecatedApisDemo()
    {
        InitializeComponent();
    }
}
