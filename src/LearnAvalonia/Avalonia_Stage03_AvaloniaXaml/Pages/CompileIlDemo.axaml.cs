// Doc:    第3部分-阶段3-Avalonia-XAML.md §8
// Title:  XAML 编译、IL 与动态加载
// Stage:  Stage03
// Accept: 说明编译进 IL 与 Loader 的 AOT 边界
// Notes:  展示编译进 IL 收益与 Loader 的 AOT 禁区。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage03_AvaloniaXaml.Pages;

public partial class CompileIlDemo : UserControl
{
    public CompileIlDemo()
    {
        InitializeComponent();
    }
}
