// Doc:    第9部分-阶段21-应用架构与工程结构.md §2
// Title:  分层 vs 垂直切片
// Stage:  Stage21
// Accept: 画清依赖方向
// Notes:  物理文件夹按功能；依赖规则仍按分层。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage21_AppArchitecture.Pages;

public partial class LayeringSlicingDemo : UserControl
{
    public LayeringSlicingDemo()
    {
        InitializeComponent();
    }
}
