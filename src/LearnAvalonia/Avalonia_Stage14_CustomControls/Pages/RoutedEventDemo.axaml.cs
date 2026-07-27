// Doc:    第6部分-阶段14-自定义控件.md §2.9
// Title:  路由事件
// Stage:  Stage14
// Accept: RoutedEvent.Register + AddHandler
// Notes:  RoutedEvent.Register + AddHandler/RemoveHandler 包装事件。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage14_CustomControls.Pages;

public partial class RoutedEventDemo : UserControl
{
    public RoutedEventDemo()
    {
        InitializeComponent();
    }
}
