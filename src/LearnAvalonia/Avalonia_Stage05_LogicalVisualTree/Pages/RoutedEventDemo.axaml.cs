// Doc:    第3部分-阶段5-LogicalTree-VisualTree-NameScope.md §6
// Title:  路由事件、焦点与 Hit Testing
// Stage:  Stage05
// Accept: 描述路由事件与 hit test 路径
// Notes:  交互式：内层按钮 Click + 外层 Border PointerPressed 观察冒泡。
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace LearnAvalonia.Avalonia_Stage05_LogicalVisualTree.Pages;

public partial class RoutedEventDemo : UserControl
{
    public RoutedEventDemo()
    {
        InitializeComponent();
    }

    private void InnerButton_Click(object? sender, RoutedEventArgs e)
    {
        if (EventLog is not null)
        {
            EventLog.Text = "事件日志：Button.Click（源=Button，冒泡）";
        }
    }

    private void OuterBorder_PointPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        if (EventLog is not null)
        {
            EventLog.Text = "事件日志：Border.PointerPressed（点击在 Border 空白处，非按钮）";
        }
    }
}
