// Doc:    第6部分-阶段14-自定义控件.md §2.13
// Title:  Design-time
// Stage:  Stage14
// Accept: Design.DataContext + Design.IsDesignMode
// Notes:  Design.DataContext 预览；Design.IsDesignMode 跳硬件。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage14_CustomControls.Pages;

public partial class DesignTimeDemo : UserControl
{
    public DesignTimeDemo()
    {
        InitializeComponent();
    }
}
