// Doc:    第10部分-Capstone4-自定义控件渲染与Native-AOT.md §1.5
// Title:  RadialPanel —— 自定义布局 Panel
// Stage:  Capstone4
// Accept: MeasureOverride + ArrangeOverride
// Notes:  把子元素均匀分布在圆周上；AffectsMeasure on Radius/StartAngle。
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace LearnAvalonia.Avalonia_Capstone4_CustomControlAOT.Pages;

public sealed class RadialPanel : Panel
{
    public static readonly StyledProperty<double> RadiusProperty =
        AvaloniaProperty.Register<RadialPanel, double>(nameof(Radius), defaultValue: 80d);

    public static readonly StyledProperty<double> StartAngleProperty =
        AvaloniaProperty.Register<RadialPanel, double>(nameof(StartAngle), defaultValue: 0d);

    static RadialPanel()
    {
        AffectsMeasure<RadialPanel>(RadiusProperty, StartAngleProperty);
    }

    public double Radius
    {
        get => GetValue(RadiusProperty);
        set => SetValue(RadiusProperty, value);
    }

    public double StartAngle
    {
        get => GetValue(StartAngleProperty);
        set => SetValue(StartAngleProperty, value);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        double maxChildW = 0;
        double maxChildH = 0;
        foreach (Control child in Children)
        {
            if (child is not Layoutable layoutable || !layoutable.IsVisible)
            {
                continue;
            }

            layoutable.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            maxChildW = Math.Max(maxChildW, layoutable.DesiredSize.Width);
            maxChildH = Math.Max(maxChildH, layoutable.DesiredSize.Height);
        }

        double diameter = (Radius + Math.Max(maxChildW, maxChildH) / 2) * 2;
        return new Size(diameter, diameter);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        int n = Children.Count;
        if (n == 0)
        {
            return finalSize;
        }

        double cx = finalSize.Width / 2;
        double cy = finalSize.Height / 2;
        double start = StartAngle * Math.PI / 180;
        int i = 0;
        foreach (Control child in Children)
        {
            if (child is not Layoutable layoutable || !layoutable.IsVisible)
            {
                continue;
            }

            double angle = start + 2 * Math.PI * i / n;
            double x = cx + Radius * Math.Cos(angle) - layoutable.DesiredSize.Width / 2;
            double y = cy + Radius * Math.Sin(angle) - layoutable.DesiredSize.Height / 2;
            layoutable.Arrange(new Rect(x, y, layoutable.DesiredSize.Width, layoutable.DesiredSize.Height));
            i++;
        }

        return finalSize;
    }
}
