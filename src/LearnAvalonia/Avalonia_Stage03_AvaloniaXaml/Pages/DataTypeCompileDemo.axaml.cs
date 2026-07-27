// Doc:    第3部分-阶段3-Avalonia-XAML.md §4
// Title:  x:DataType 与 x:CompileBindings
// Stage:  Stage03
// Accept: 说明 DataType 作用域与编译绑定开关
// Notes:  展示 DataType 必要性、作用域嵌套与局部关闭。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage03_AvaloniaXaml.Pages;

public partial class DataTypeCompileDemo : UserControl
{
    public DataTypeCompileDemo()
    {
        InitializeComponent();
    }
}
