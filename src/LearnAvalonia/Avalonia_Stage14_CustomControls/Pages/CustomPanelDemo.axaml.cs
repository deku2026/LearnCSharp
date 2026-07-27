// Doc:    第6部分-阶段14-自定义控件.md §2.2
// Title:  自定义 Panel
// Stage:  Stage14
// Accept: MeasureOverride/ArrangeOverride
// Notes:  Panel 只做 Measure/Arrange；测试无限 availableSize 不 NaN。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage14_CustomControls.Pages;

public partial class CustomPanelDemo : UserControl
{
    public CustomPanelDemo()
    {
        InitializeComponent();
    }
}
