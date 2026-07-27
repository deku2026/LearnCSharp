// Doc:    第3部分-阶段5-LogicalTree-VisualTree-NameScope.md §9
// Title:  TopLevel、IPresentationSource 与 12 禁令
// Stage:  Stage05
// Accept: 正确调用 GetTopLevel，不盲转
// Notes:  展示 TopLevel.GetTopLevel 正确写法与 12 禁令。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage05_LogicalVisualTree.Pages;

public partial class TopLevelDemo : UserControl
{
    public TopLevelDemo()
    {
        InitializeComponent();
    }
}
