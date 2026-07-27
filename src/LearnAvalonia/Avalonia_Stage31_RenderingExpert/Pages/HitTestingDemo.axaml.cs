// Doc:    第11部分-阶段31-渲染专家档.md §4
// Title:  Hit Testing / 命中测试
// Stage:  Stage31
// Accept: 演示命中测试非矩形
// Notes:  命中测试走 Visual Tree 而非像素；PointerPressed 时按几何判断更新 Fill 反映命中。
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;

namespace LearnAvalonia.Avalonia_Stage31_RenderingExpert.Pages;

public partial class HitTestingDemo : UserControl
{
    public HitTestingDemo()
    {
        InitializeComponent();
    }
}

public sealed class EllipseHitControl : Control
{
    static EllipseHitControl()
    {
        AffectsRender<EllipseHitControl>(FillProperty, IsHitProperty);
    }

    public static readonly StyledProperty<IBrush?> FillProperty =
        AvaloniaProperty.Register<EllipseHitControl, IBrush?>(nameof(Fill), defaultValue: Brushes.CornflowerBlue);

    public static readonly StyledProperty<bool> IsHitProperty =
        AvaloniaProperty.Register<EllipseHitControl, bool>(nameof(IsHit));

    public IBrush? Fill
    {
        get => GetValue(FillProperty);
        set => SetValue(FillProperty, value);
    }

    public bool IsHit
    {
        get => GetValue(IsHitProperty);
        set => SetValue(IsHitProperty, value);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        context.DrawEllipse(Fill, null, new Rect(Bounds.Size));
        if (IsHit)
        {
            Pen pen = new(Brushes.Yellow, 3);
            context.DrawEllipse(null, pen, new Rect(2, 2, Bounds.Width - 4, Bounds.Height - 4));
        }
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        Point pt = e.GetPosition(this);
        Size sz = Bounds.Size;
        double cx = sz.Width / 2;
        double cy = sz.Height / 2;
        double rx = cx;
        double ry = cy;
        double dx = (pt.X - cx) / rx;
        double dy = (pt.Y - cy) / ry;
        IsHit = (dx * dx + dy * dy) <= 1;
        e.Handled = true;
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        IsHit = false;
    }
}
