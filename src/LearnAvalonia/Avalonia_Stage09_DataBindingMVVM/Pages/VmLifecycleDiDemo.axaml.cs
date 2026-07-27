// Doc:    第4部分-阶段9-数据绑定与MVVM.md §E1
// Title:  VM 生命周期与 DI
// Stage:  Stage09
// Accept: 写 DI 组合根注册 VM/服务
// Notes:  创建（DI）→ 注入服务 → 激活/首次导航 LoadAsync → 失活取消 CTS → Dispose 释放。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage09_DataBindingMVVM.Pages;

public partial class VmLifecycleDiDemo : UserControl
{
    public VmLifecycleDiDemo()
    {
        InitializeComponent();
    }
}
