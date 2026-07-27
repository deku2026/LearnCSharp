// Doc:    第9部分-阶段21-应用架构与工程结构.md §1
// Title:  UI 架构选型 MVVM/Rx/MVU
// Stage:  Stage21
// Accept: 书面选型并给理由
// Notes:  默认 CommunityToolkit MVVM；复杂流评估 ReactiveUI；强约束单向可试点 MVU。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage21_AppArchitecture.Pages;

public partial class UiArchitectureChoiceDemo : UserControl
{
    public UiArchitectureChoiceDemo()
    {
        InitializeComponent();
    }
}
