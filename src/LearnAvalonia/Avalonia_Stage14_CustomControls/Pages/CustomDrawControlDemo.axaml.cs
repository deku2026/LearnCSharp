// Doc:    第6部分-阶段14-自定义控件.md §2.3
// Title:  自定义绘制 Control
// Stage:  Stage14
// Accept: StyledProperty+AffectsRender+AutomationPeer
// Notes:  Control+Render 自绘；AffectsRender/AffectsMeasure/HitTestCore。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage14_CustomControls.Pages;

public partial class CustomDrawControlDemo : UserControl
{
    public CustomDrawControlDemo()
    {
        InitializeComponent();
    }
}
