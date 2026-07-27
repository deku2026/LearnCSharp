// Doc:    第4部分-阶段9-数据绑定与MVVM.md §B1
// Title:  Compiled vs Reflection
// Stage:  Stage09
// Accept: 解释 compiled vs reflection 与 x:DataType
// Notes:  12 默认倾向编译绑定；x:DataType 是编译绑定的类型上下文。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage09_DataBindingMVVM.Pages;

public partial class CompiledReflectionDemo : UserControl
{
    public CompiledReflectionDemo()
    {
        InitializeComponent();
    }
}
