// Doc:    第2部分-阶段2-Dispatcher线程与UI亲和性.md §5
// Title:  多 Dispatcher，但仍是单 UI 线程模型
// Stage:  Stage02
// Accept: 默写每线程 Dispatcher；多 UI 线程不支持
// Notes:  展示多 Dispatcher 边界与 DispatcherTimer 当前 dispatcher 绑定。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage02_Dispatcher.Pages;

public partial class MultiDispatcherDemo : UserControl
{
    public MultiDispatcherDemo()
    {
        InitializeComponent();
    }
}
