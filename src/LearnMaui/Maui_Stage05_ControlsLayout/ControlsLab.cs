using Microsoft.Maui.Layouts;

namespace LearnMaui.Maui_Stage05_ControlsLayout;

public sealed class TwoColumnLayout : Microsoft.Maui.Controls.Layout
{
    public static readonly BindableProperty SpacingProperty = BindableProperty.Create(
        nameof(Spacing),
        typeof(double),
        typeof(TwoColumnLayout),
        8d,
        propertyChanged: static (bindable, _, _) =>
            ((TwoColumnLayout)bindable).InvalidateMeasure());

    public double Spacing
    {
        get => (double)GetValue(SpacingProperty);
        set => SetValue(SpacingProperty, value);
    }

    protected override ILayoutManager CreateLayoutManager()
    {
        return new TwoColumnLayoutManager(this);
    }
}

public sealed class TwoColumnLayoutManager(TwoColumnLayout layout) : LayoutManager(layout)
{
    private readonly TwoColumnLayout _layout = layout;

    public override Size Measure(double widthConstraint, double heightConstraint)
    {
        Thickness padding = _layout.Padding;
        double width = double.IsInfinity(widthConstraint) ? 400 : widthConstraint;
        double columnWidth = Math.Max(
            0,
            (width - padding.HorizontalThickness - _layout.Spacing) / 2);
        double[] heights = [0, 0];
        int index = 0;

        foreach (IView child in _layout)
        {
            if (child.Visibility == Visibility.Collapsed)
            {
                continue;
            }

            Size desired = child.Measure(columnWidth, heightConstraint);
            heights[index % 2] += desired.Height + _layout.Spacing;
            index++;
        }

        double height = Math.Max(heights[0], heights[1]);
        if (index > 0)
        {
            height -= _layout.Spacing;
        }

        return new Size(width, Math.Max(0, height + padding.VerticalThickness));
    }

    public override Size ArrangeChildren(Rect bounds)
    {
        Thickness padding = _layout.Padding;
        double columnWidth = Math.Max(
            0,
            (bounds.Width - padding.HorizontalThickness - _layout.Spacing) / 2);
        double[] y = [bounds.Top + padding.Top, bounds.Top + padding.Top];
        double[] x =
        [
            bounds.Left + padding.Left,
            bounds.Left + padding.Left + columnWidth + _layout.Spacing,
        ];
        int index = 0;

        foreach (IView child in _layout)
        {
            if (child.Visibility == Visibility.Collapsed)
            {
                continue;
            }

            int column = index % 2;
            child.Arrange(new Rect(x[column], y[column], columnWidth, child.DesiredSize.Height));
            y[column] += child.DesiredSize.Height + _layout.Spacing;
            index++;
        }

        double height = Math.Max(y[0], y[1]) - bounds.Top;
        if (index > 0)
        {
            height -= _layout.Spacing;
        }

        return new Size(bounds.Width, height + padding.Bottom);
    }
}

public abstract record Widget(string Id);

public sealed record TextWidget(string Id, string Text) : Widget(Id);

public sealed record ChartWidget(string Id, IReadOnlyList<double> Values) : Widget(Id);

public sealed record ImageWidget(string Id, string Image) : Widget(Id);

public sealed class WidgetTemplateSelector : DataTemplateSelector
{
    public DataTemplate TextTemplate { get; set; } = new(() => new Label());

    public DataTemplate ChartTemplate { get; set; } = new(() => new GraphicsView());

    public DataTemplate ImageTemplate { get; set; } = new(() => new Image());

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        return item switch
        {
            ChartWidget => ChartTemplate,
            ImageWidget => ImageTemplate,
            _ => TextTemplate,
        };
    }
}

public static class ControlsFactory
{
    public static IReadOnlyList<View> CreateControlMatrix()
    {
        return
        [
            new Border { Content = new Label { Text = "Border" } },
            new Button { Text = "Button" },
            new Image { Source = "appicon.svg" },
            new Entry { Placeholder = "Entry" },
            new Editor { Placeholder = "Editor" },
            new SearchBar { Placeholder = "SearchBar" },
            new Picker { ItemsSource = new[] { "One", "Two" } },
            new DatePicker(),
            new TimePicker(),
            new CheckBox(),
            new RadioButton { Content = "Radio" },
            new Switch(),
            new Slider(),
            new Stepper(),
            new ActivityIndicator { IsRunning = true },
            new ProgressBar { Progress = 0.6 },
            new CollectionView { ItemsSource = new[] { "A", "B" } },
            new CarouselView { ItemsSource = new[] { "A", "B" } },
            new SwipeView { Content = new Label { Text = "Swipe" } },
            new RefreshView { Content = new Label { Text = "Refresh" } },
            new ScrollView { Content = new Label { Text = "Scroll" } },
            new WebView { Source = new HtmlWebViewSource { Html = "<p>WebView</p>" } },
            new GraphicsView(),
        ];
    }

    public static int GetAdaptiveColumns(double width)
    {
        return width switch
        {
            < 600 => 1,
            < 1000 => 2,
            _ => 3,
        };
    }
}
