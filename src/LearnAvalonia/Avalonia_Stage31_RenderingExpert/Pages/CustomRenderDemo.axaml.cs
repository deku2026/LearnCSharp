// Doc:    第11部分-阶段31-渲染专家档.md §16
// Title:  自定义 Render 示例
// Stage:  Stage31
// Accept: 写 Control + Render 自绘
// Notes:  下方的 CustomDrawnControl 重写 Render(DrawingContext) 画圆+矩形+线+文本。
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace LearnAvalonia.Avalonia_Stage31_RenderingExpert.Pages;

public partial class CustomRenderDemo : UserControl
{
    public CustomRenderDemo()
    {
        InitializeComponent();
    }
}

public sealed class CustomDrawnControl : Control
{
    static CustomDrawnControl()
    {
        AffectsRender<CustomDrawnControl>(FillProperty);
    }

    public static readonly StyledProperty<IBrush?> FillProperty =
        AvaloniaProperty.Register<CustomDrawnControl, IBrush?>(nameof(Fill), defaultValue: Brushes.MediumSeaGreen);

    public IBrush? Fill
    {
        get => GetValue(FillProperty);
        set => SetValue(FillProperty, value);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        Rect r = Bounds;
        double w = r.Width;
        double h = r.Height;
        context.DrawRectangle(Brushes.Bisque, null, new Rect(0, 0, w, h));
        context.DrawEllipse(Fill, null, new Rect(8, 8, w - 16, h - 16));
        Pen pen = new(Brushes.DarkRed, 2);
        context.DrawLine(pen, new Point(0, 0), new Point(w, h));
        context.DrawLine(pen, new Point(w, 0), new Point(0, h));
        FormattedText fmt = new(
            "Render(DrawingContext) 自绘",
            System.Globalization.CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            Typeface.Default,
            16,
            null);
        context.DrawText(fmt, new Point(8, h - 22));
    }
}
