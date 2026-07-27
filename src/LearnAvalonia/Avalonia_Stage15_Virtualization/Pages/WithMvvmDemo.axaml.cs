// Doc:    第6部分-阶段15-虚拟化与大数据UI.md §2.11
// Title:  与 MVVM
// Stage:  Stage15
// Accept: 虚拟化下 MVVM 注意点
// Notes:  ObservableCollection 增删 O(log n) 通知；避免 N 项 Reset；容器复用下绑定状态隔离。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage15_Virtualization.Pages;

public partial class WithMvvmDemo : UserControl
{
    public WithMvvmDemo()
    {
        InitializeComponent();
    }
}
