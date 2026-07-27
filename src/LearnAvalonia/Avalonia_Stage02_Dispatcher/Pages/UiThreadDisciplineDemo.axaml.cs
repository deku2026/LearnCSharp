// Doc:    第2部分-阶段2-Dispatcher线程与UI亲和性.md §1
// Title:  为什么 UI 线程纪律决定稳定性
// Stage:  Stage02
// Accept: 说明事故成因与 lifetime 关系
// Notes:  展示 5 类典型事故与阶段 1 lifetime 的关系。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage02_Dispatcher.Pages;

public partial class UiThreadDisciplineDemo : UserControl
{
    public UiThreadDisciplineDemo()
    {
        InitializeComponent();
    }
}
