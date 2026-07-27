// Doc:    第5部分-阶段12-输入焦点手势与IME.md §2.1
// Title:  路由事件：direct / tunnel / bubble
// Stage:  Stage12
// Accept: 演示 Handled 与 handledEventsToo
// Notes:  Tunnel/Bubble/Direct 三策略；Handled 停止路由。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage12_InputFocusGestures.Pages;

public partial class RoutedEventDemo : UserControl
{
    public RoutedEventDemo()
    {
        InitializeComponent();
    }
}
