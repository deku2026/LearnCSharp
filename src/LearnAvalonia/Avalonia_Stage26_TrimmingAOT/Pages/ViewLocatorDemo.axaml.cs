// Doc:    第10部分-阶段26-Trimming与Native-AOT.md §5
// Title:  ViewLocator 不友好
// Stage:  Stage26
// Accept: ViewLocator 反射 vs CompiledBinding
// Notes:  ViewLocator 用反射 Build.ViewModel→View；AOT 下需 GeneratedViewLocator。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage26_TrimmingAOT.Pages;

public partial class ViewLocatorDemo : UserControl
{
    public ViewLocatorDemo()
    {
        InitializeComponent();
    }
}

internal static class ReflectionBasedDemo
{
    // ❌ 反射路径：AOT 下失败
    public static Avalonia.Controls.Control BuildByReflection(object viewModel)
    {
        Type viewModelType = viewModel.GetType();
        string viewName = viewModelType.FullName!.Replace("ViewModel", "View");
        Type? viewType = Type.GetType(viewName);
        return viewType is not null
            ? (Avalonia.Controls.Control)Activator.CreateInstance(viewType)!
            : new TextBlock { Text = "view not found" };
    }
}

internal sealed record CompiledBindingDemoVm(string Title);

internal sealed class CompiledBindingDemoView : UserControl
{
    public CompiledBindingDemoView(CompiledBindingDemoVm vm)
    {
        Content = new TextBlock { Text = vm.Title };
    }
}

internal static class CompiledBindingDemo
{
    // ✅ 编译期已知：AOT 友好
    public static CompiledBindingDemoView Build(CompiledBindingDemoVm vm) => new(vm);
}
