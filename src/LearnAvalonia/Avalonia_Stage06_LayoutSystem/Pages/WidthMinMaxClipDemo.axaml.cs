// Doc:    第4部分-阶段6-布局系统.md §C3
// Title:  Width/Height 与 Min/Max 与 Clip
// Stage:  Stage06
// Accept: 说明 Width/Min/Max 优先级与 Clip
// Notes:  NaN=Auto 语义；MaxWidth=0 等极端值配合 IsVisible/Opacity。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage06_LayoutSystem.Pages;

public partial class WidthMinMaxClipDemo : UserControl
{
    public WidthMinMaxClipDemo()
    {
        InitializeComponent();
    }
}
