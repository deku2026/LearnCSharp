// Doc:    第4部分-阶段9-数据绑定与MVVM.md §B3
// Title:  代码侧绑定与何时 Reflection
// Stage:  Stage09
// Accept: 说清 CompiledBinding.Create vs ReflectionBinding
// Notes:  代码里 new Binding(...) 语义在 12 更接近反射绑定；BindingBase 作为绑定抽象基类型。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage09_DataBindingMVVM.Pages;

public partial class CodeBindingDemo : UserControl
{
    public CodeBindingDemo()
    {
        InitializeComponent();
    }
}
