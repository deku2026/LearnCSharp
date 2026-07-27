// Doc:    第11部分-阶段31-渲染专家档.md §6
// Title:  Skia 主线
// Stage:  Stage31
// Accept: 画出 Draw 调用栈
// Notes:  DrawingContext → SkiaDrawingContextImpl → SKCanvas；12 后唯一主渲染器。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage31_RenderingExpert.Pages;

public partial class SkiaMainDemo : UserControl
{
    public SkiaMainDemo()
    {
        InitializeComponent();
    }
}
