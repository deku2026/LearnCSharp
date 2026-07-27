// Doc:    第9部分-阶段21-应用架构与工程结构.md §3
// Title:  模块化与贡献点
// Stage:  Stage21
// Accept: IModule + 贡献点契约
// Notes:  稳定的是契约程序集；实现可替换。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage21_AppArchitecture.Pages;

public partial class ModularizationDemo : UserControl
{
    public ModularizationDemo()
    {
        InitializeComponent();
    }
}
