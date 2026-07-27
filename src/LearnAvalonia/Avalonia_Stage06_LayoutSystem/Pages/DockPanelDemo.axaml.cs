// Doc:    第4部分-阶段6-布局系统.md §D5
// Title:  DockPanel
// Stage:  Stage06
// Accept: 使用 Dock 与 LastChildFill
// Notes:  顺序敏感：先声明的先占边；末子 Fill 得剩余区域。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage06_LayoutSystem.Pages;

public partial class DockPanelDemo : UserControl
{
    public DockPanelDemo()
    {
        InitializeComponent();
    }
}
