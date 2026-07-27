// Doc:    第2部分-阶段2-Dispatcher线程与UI亲和性.md §4
// Title:  Yield、Resume 与 SynchronizationContext
// Stage:  Stage02
// Accept: 理解 AvaloniaSynchronizationContext 与 ConfigureAwait
// Notes:  展示 SyncContext 行为、ConfigureAwait 边界与 Yield 协作让出。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage02_Dispatcher.Pages;

public partial class YieldResumeDemo : UserControl
{
    public YieldResumeDemo()
    {
        InitializeComponent();
    }
}
