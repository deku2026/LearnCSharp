// Doc:    第7部分-阶段16-统一平台抽象.md §8
// Title:  拖放（DnD）
// Stage:  Stage16
// Accept: 应用内 vs 应用外 DnD
// Notes:  DragDrop.SetAllowDrop；DragDrop.DragEnter/Leave/Over/Drop 事件；DataObject。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage16_PlatformAbstraction.Pages;

public partial class DragDropDemo : UserControl
{
    public DragDropDemo()
    {
        InitializeComponent();
    }
}
