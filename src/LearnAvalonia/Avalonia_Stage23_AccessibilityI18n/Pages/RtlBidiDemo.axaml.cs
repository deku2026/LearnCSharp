// Doc:    第10部分-阶段23-无障碍国际化与文本.md §5
// Title:  RTL、BiDi 与布局镜像
// Stage:  Stage23
// Accept: FlowDirection 切换可演示
// Notes:  FlowDirection=RightToLeft；LayoutTransform 镜像图标；BiDi 文本算法 Unicode Bidirectional。
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace LearnAvalonia.Avalonia_Stage23_AccessibilityI18n.Pages;

public partial class RtlBidiDemo : UserControl
{
    public RtlBidiDemo()
    {
        InitializeComponent();
    }

    private void ToggleRtl_Click(object? sender, RoutedEventArgs e)
    {
        if (RtlTarget is not null)
        {
            RtlTarget.FlowDirection = RtlTarget.FlowDirection == FlowDirection.LeftToRight
                ? FlowDirection.RightToLeft
                : FlowDirection.LeftToRight;
        }

        if (DirectionStatus is not null)
        {
            DirectionStatus.Text = "当前 FlowDirection=" + (RtlTarget?.FlowDirection.ToString() ?? "?");
        }
    }
}
