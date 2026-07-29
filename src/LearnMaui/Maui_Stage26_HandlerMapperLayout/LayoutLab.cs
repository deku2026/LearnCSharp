using Microsoft.Maui.Layouts;

namespace LearnMaui.Maui_Stage26_HandlerMapperLayout;

public sealed record LayoutPass(
    string Kind,
    double Width,
    double Height,
    int VisibleChildren);

public sealed class TraceColumnLayout : Microsoft.Maui.Controls.Layout
{
    public static readonly BindableProperty SpacingProperty = BindableProperty.Create(
        nameof(Spacing),
        typeof(double),
        typeof(TraceColumnLayout),
        8d,
        validateValue: static (_, value) => (double)value >= 0,
        propertyChanged: static (bindable, _, _) =>
            ((TraceColumnLayout)bindable).InvalidateMeasure());

    private readonly Queue<LayoutPass> _passes = new();

    public double Spacing
    {
        get => (double)GetValue(SpacingProperty);
        set => SetValue(SpacingProperty, value);
    }

    public IReadOnlyList<LayoutPass> Snapshot() => _passes.ToArray();

    internal void Record(LayoutPass pass)
    {
        _passes.Enqueue(pass);
        while (_passes.Count > 20)
        {
            _passes.Dequeue();
        }
    }

    protected override ILayoutManager CreateLayoutManager() =>
        new TraceColumnLayoutManager(this);
}

public sealed class TraceColumnLayoutManager(TraceColumnLayout layout)
    : LayoutManager(layout)
{
    private readonly TraceColumnLayout _layout = layout;

    public override Size Measure(double widthConstraint, double heightConstraint)
    {
        Thickness padding = _layout.Padding;
        double childWidthConstraint = double.IsInfinity(widthConstraint)
            ? double.PositiveInfinity
            : Math.Max(0, widthConstraint - padding.HorizontalThickness);
        double measuredWidth = 0;
        double measuredHeight = padding.VerticalThickness;
        int visibleCount = 0;

        foreach (IView child in _layout)
        {
            if (child.Visibility == Visibility.Collapsed)
            {
                continue;
            }

            Size desired = child.Measure(
                childWidthConstraint,
                double.PositiveInfinity);
            measuredWidth = Math.Max(measuredWidth, desired.Width);
            measuredHeight += desired.Height;
            if (visibleCount > 0)
            {
                measuredHeight += _layout.Spacing;
            }

            visibleCount++;
        }

        double finalWidth = double.IsInfinity(widthConstraint)
            ? measuredWidth + padding.HorizontalThickness
            : widthConstraint;
        double finalHeight = double.IsInfinity(heightConstraint)
            ? measuredHeight
            : Math.Min(measuredHeight, heightConstraint);
        _layout.Record(new LayoutPass(
            "measure",
            widthConstraint,
            heightConstraint,
            visibleCount));
        return new Size(Math.Max(0, finalWidth), Math.Max(0, finalHeight));
    }

    public override Size ArrangeChildren(Rect bounds)
    {
        Thickness padding = _layout.Padding;
        double x = bounds.Left + padding.Left;
        double y = bounds.Top + padding.Top;
        double width = Math.Max(0, bounds.Width - padding.HorizontalThickness);
        int visibleCount = 0;

        foreach (IView child in _layout)
        {
            if (child.Visibility == Visibility.Collapsed)
            {
                continue;
            }

            double height = child.DesiredSize.Height;
            child.Arrange(new Rect(x, y, width, height));
            y += height + _layout.Spacing;
            visibleCount++;
        }

        if (visibleCount > 0)
        {
            y -= _layout.Spacing;
        }

        _layout.Record(new LayoutPass(
            "arrange",
            bounds.Width,
            bounds.Height,
            visibleCount));
        return new Size(bounds.Width, Math.Max(0, y - bounds.Top + padding.Bottom));
    }
}

public static class ConstraintLab
{
    public static (double Width, double Height) Normalize(
        double widthConstraint,
        double heightConstraint,
        double fallbackWidth)
    {
        double width = double.IsInfinity(widthConstraint)
            ? fallbackWidth
            : Math.Max(0, widthConstraint);
        double height = double.IsInfinity(heightConstraint)
            ? double.PositiveInfinity
            : Math.Max(0, heightConstraint);
        return (width, height);
    }
}
