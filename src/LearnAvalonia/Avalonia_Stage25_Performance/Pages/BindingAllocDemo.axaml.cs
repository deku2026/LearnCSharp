// Doc:    第10部分-阶段25-性能与诊断.md §4
// Title:  绑定与分配
// Stage:  Stage25
// Accept: CompiledBinding 减少 alloc
// Notes:  CompiledBinding 编译期生成；ReflectionBinding 运行时反射 + 装箱。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage25_Performance.Pages;

public partial class BindingAllocDemo : UserControl
{
    public BindingAllocDemo()
    {
        InitializeComponent();
    }
}
