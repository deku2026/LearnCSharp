// Doc:    第3部分-阶段5-LogicalTree-VisualTree-NameScope.md §12
// Title:  综合示例：自定义带模板的控件与树
// Stage:  Stage05
// Accept: 自定义带模板控件的树
// Notes:  展示 LabeledField TemplatedControl 与 OnApplyTemplate 取 PART。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage05_LogicalVisualTree.Pages;

public partial class LabeledFieldDemo : UserControl
{
    public LabeledFieldDemo()
    {
        InitializeComponent();
    }
}
