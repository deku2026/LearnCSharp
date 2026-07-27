// Doc:    第2部分-阶段2-Dispatcher线程与UI亲和性.md §8
// Title:  与 async/await 的关系及死锁场景清单
// Stage:  Stage02
// Accept: 列出至少 3 种死锁/卡顿禁令（含剪贴板回调）
// Notes:  展示 7 行死锁/卡顿场景表与诊断思路。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage02_Dispatcher.Pages;

public partial class AsyncDeadlockDemo : UserControl
{
    public AsyncDeadlockDemo()
    {
        InitializeComponent();
    }
}
