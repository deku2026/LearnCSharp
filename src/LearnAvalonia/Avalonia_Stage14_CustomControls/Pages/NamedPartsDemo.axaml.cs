// Doc:    第6部分-阶段14-自定义控件.md §2.6
// Title:  Named parts 约定
// Stage:  Stage14
// Accept: PART_ 前缀；卸载旧事件
// Notes:  PART_ 前缀契约；OnApplyTemplate 先解绑再绑。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage14_CustomControls.Pages;

public partial class NamedPartsDemo : UserControl
{
    public NamedPartsDemo()
    {
        InitializeComponent();
    }
}
