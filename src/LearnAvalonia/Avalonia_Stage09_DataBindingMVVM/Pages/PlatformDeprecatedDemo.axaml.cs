// Doc:    第4部分-阶段9-数据绑定与MVVM.md §J1
// Title:  平台差·过时·反模式
// Stage:  Stage09
// Accept: 禁止推荐 Avalonia.ReactiveUI 11.x
// Notes:  桌面多窗每窗 VM 实例；移动 Pages 导航参数与 VM 生命周期绑定；Browser 单页注意刷新丢状态。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage09_DataBindingMVVM.Pages;

public partial class PlatformDeprecatedDemo : UserControl
{
    public PlatformDeprecatedDemo()
    {
        InitializeComponent();
    }
}
