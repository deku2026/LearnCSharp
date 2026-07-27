// Doc:    第2部分-阶段2-Dispatcher线程与UI亲和性.md §2
// Title:  Dispatcher 家族 API
// Stage:  Stage02
// Accept: 讲清 UIThread/CurrentDispatcher/AvaloniaObject.Dispatcher
// Notes:  展示 4 个 Dispatcher 获取入口与 CheckAccess/VerifyAccess。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage02_Dispatcher.Pages;

public partial class DispatcherFamilyDemo : UserControl
{
    public DispatcherFamilyDemo()
    {
        InitializeComponent();
    }
}
