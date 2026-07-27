// Doc:    第3部分-阶段5-LogicalTree-VisualTree-NameScope.md §4
// Title:  NameScope：名字的边界
// Stage:  Stage05
// Accept: 解释模板内外 x:Name 互不可见
// Notes:  展示 NameScope 概念、FindControl 与模板独立 NameScope。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage05_LogicalVisualTree.Pages;

public partial class NameScopeDemo : UserControl
{
    public NameScopeDemo()
    {
        InitializeComponent();
    }
}
