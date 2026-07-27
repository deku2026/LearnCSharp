// Doc:    第4部分-阶段6-布局系统.md §F1
// Title:  自定义 Panel：SimpleStackPanel
// Stage:  Stage06
// Accept: 提交一个自定义 Panel + 布局断言
// Notes:  实现 MeasureOverride/ArrangeOverride；属性变更时 InvalidateMeasure。
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace LearnAvalonia.Avalonia_Stage06_LayoutSystem.Pages;

public partial class CustomPanelDemo : UserControl
{
    public CustomPanelDemo()
    {
        InitializeComponent();
    }
}

public sealed class SimpleStackPanel : Panel
{
    public static readonly StyledProperty<double> SpacingProperty =
        AvaloniaProperty.Register<SimpleStackPanel, double>(nameof(Spacing));

    public double Spacing
    {
        get => GetValue(SpacingProperty);
        set => SetValue(SpacingProperty, value);
    }

    static SimpleStackPanel() => AffectsMeasure<SimpleStackPanel>(SpacingProperty);

    protected override Size MeasureOverride(Size availableSize)
    {
        double w = 0, h = 0;
        bool first = true;
        foreach (Control child in Children)
        {
            if (child is not Layoutable layoutable || !layoutable.IsVisible)
            {
                continue;
            }

            layoutable.Measure(new Size(availableSize.Width, double.PositiveInfinity));
            w = Math.Max(w, layoutable.DesiredSize.Width);
            h += layoutable.DesiredSize.Height + (first ? 0 : Spacing);
            first = false;
        }

        return new Size(w, h);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        double y = 0;
        bool first = true;
        foreach (Control child in Children)
        {
            if (child is not Layoutable layoutable || !layoutable.IsVisible)
            {
                continue;
            }

            if (!first)
            {
                y += Spacing;
            }

            first = false;
            layoutable.Arrange(new Rect(0, y, finalSize.Width, layoutable.DesiredSize.Height));
            y += layoutable.DesiredSize.Height;
        }

        return finalSize;
    }
}
