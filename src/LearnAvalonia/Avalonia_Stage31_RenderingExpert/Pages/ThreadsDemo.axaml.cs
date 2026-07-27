// Doc:    第11部分-阶段31-渲染专家档.md §12
// Title:  UI 线程 vs 渲染线程
// Stage:  Stage31
// Accept: 解释同步与可取消
// Notes:  Compositor 在独立线程；UI 线程更新 Visual 后提交。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage31_RenderingExpert.Pages;

public partial class ThreadsDemo : UserControl
{
    public ThreadsDemo()
    {
        InitializeComponent();
    }
}
