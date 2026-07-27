// Doc:    第10部分-阶段24-测试体系.md §2
// Title:  Unit：ViewModel 与服务
// Stage:  Stage24
// Accept: xUnit 单测 ViewModel
// Notes:  纯逻辑单测，不需要 AvaloniaFact；测试 ObservableProperty 命令触发。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage24_TestingSystem.Pages;

public partial class UnitViewModelDemo : UserControl
{
    public UnitViewModelDemo()
    {
        InitializeComponent();
    }
}
