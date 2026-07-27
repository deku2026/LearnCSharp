// Doc:    第10部分-Capstone4-自定义控件渲染与Native-AOT.md §1.1
// Title:  GaugeControl —— TemplatedControl 教学版
// Stage:  Capstone4
// Accept: 运行 GaugeControl + Value 变更
// Notes:  含 StyledProperty (Value/Maximum) + DirectProperty (NormalizedRatio) + 路由事件 ValueChanged
//         + ControlTheme 在 App.axaml + 自定义 Render(DrawingContext) + AutomationPeer + 动画。
using Avalonia;
using Avalonia.Automation;
using Avalonia.Automation.Peers;
using Avalonia.Automation.Provider;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace LearnAvalonia.Avalonia_Capstone4_CustomControlAOT.Pages;

public class GaugeControl : TemplatedControl
{
    public static readonly StyledProperty<double> ValueProperty =
        AvaloniaProperty.Register<GaugeControl, double>(nameof(Value), defaultValue: 0d);

    public static readonly StyledProperty<double> MaximumProperty =
        AvaloniaProperty.Register<GaugeControl, double>(nameof(Maximum), defaultValue: 100d);

    public static readonly StyledProperty<IBrush?> NeedleBrushProperty =
        AvaloniaProperty.Register<GaugeControl, IBrush?>(nameof(NeedleBrush), defaultValue: Brushes.OrangeRed);

    public static readonly RoutedEvent<ValueChangedEventArgs> ValueChangedEvent =
        RoutedEvent.Register<GaugeControl, ValueChangedEventArgs>(nameof(ValueChanged), RoutingStrategies.Bubble);

    private double _normalizedRatio;

    static GaugeControl()
    {
        ValueProperty.Changed.AddClassHandler<GaugeControl>((g, e) => g.OnValueChanged(e));
        MaximumProperty.Changed.AddClassHandler<GaugeControl>((g, e) => g.UpdateNormalizedRatio());
        AffectsRender<GaugeControl>(ValueProperty, MaximumProperty, NeedleBrushProperty);
    }

    public GaugeControl()
    {
        UpdateNormalizedRatio();
    }

    public double Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public double Maximum
    {
        get => GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    public IBrush? NeedleBrush
    {
        get => GetValue(NeedleBrushProperty);
        set => SetValue(NeedleBrushProperty, value);
    }

    public double NormalizedRatio
    {
        get => _normalizedRatio;
        private set => SetAndRaise(NormalizedRatioProperty, ref _normalizedRatio, value);
    }

    public static readonly DirectProperty<GaugeControl, double> NormalizedRatioProperty =
        AvaloniaProperty.RegisterDirect<GaugeControl, double>(nameof(NormalizedRatio),
            o => o.NormalizedRatio, (o, v) => o.NormalizedRatio = v);

    public event EventHandler<ValueChangedEventArgs>? ValueChanged
    {
        add => AddHandler(ValueChangedEvent, value);
        remove => RemoveHandler(ValueChangedEvent, value);
    }

    protected virtual void OnValueChanged(AvaloniaPropertyChangedEventArgs e)
    {
        UpdateNormalizedRatio();
        if (e.OldValue is double old && e.NewValue is double now)
        {
            RaiseEvent(new ValueChangedEventArgs(ValueChangedEvent, old, now));
        }
    }

    private void UpdateNormalizedRatio()
    {
        double max = Maximum;
        NormalizedRatio = max <= 0 ? 0 : Math.Clamp(Value / max, 0d, 1d);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        Size sz = Bounds.Size;
        double w = sz.Width;
        double h = sz.Height;
        double side = Math.Min(w, h) - 8;
        Rect bounds = new(4, 4, side, side);
        context.DrawEllipse(null, new Pen(Brushes.Gray, 2), bounds);
        double startAngle = 135 * Math.PI / 180;
        double sweepAngle = 270 * NormalizedRatio * Math.PI / 180;
        Pen valuePen = new(NeedleBrush ?? Brushes.OrangeRed, 6);
        double cx = bounds.X + bounds.Width / 2;
        double cy = bounds.Y + bounds.Height / 2;
        double r = side / 2;
        Point p0 = new(cx + r * Math.Cos(startAngle), cy + r * Math.Sin(startAngle));
        Point p1 = new(cx + r * Math.Cos(startAngle + sweepAngle), cy + r * Math.Sin(startAngle + sweepAngle));
        // 用 PathGeometry 画圆弧（DrawArc 在 Avalonia 12 DrawingContext 不存在）
        PathFigure fig = new()
        {
            StartPoint = p0,
            IsClosed = false,
            Segments = new PathSegments
            {
                new ArcSegment
                {
                    Size = new Size(side / 2, side / 2),
                    IsLargeArc = sweepAngle > Math.PI,
                    SweepDirection = SweepDirection.Clockwise,
                    Point = p1,
                },
            },
        };
        PathGeometry geo = new() { Figures = new PathFigures { fig } };
        context.DrawGeometry(null, valuePen, geo);

        // 刻度
        for (int i = 0; i <= 10; i++)
        {
            double a = startAngle + 270 * (i / 10d) * Math.PI / 180;
            Point o = new(cx + (r - 4) * Math.Cos(a), cy + (r - 4) * Math.Sin(a));
            Point inner = new(cx + (r - 12) * Math.Cos(a), cy + (r - 12) * Math.Sin(a));
            context.DrawLine(new Pen(Brushes.Black, 1), inner, o);
        }

        FormattedText fmt = new(
            Value.ToString("0"),
            System.Globalization.CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            Typeface.Default,
            18,
            null);
        context.DrawText(fmt, new Point(cx - fmt.Width / 2, cy - fmt.Height / 2));
    }

    protected override AutomationPeer OnCreateAutomationPeer()
        => new GaugeAutomationPeer(this);

    private sealed class GaugeAutomationPeer : ControlAutomationPeer, IRangeValueProvider
    {
        public GaugeAutomationPeer(GaugeControl owner) : base(owner) { }

        private GaugeControl G => (GaugeControl)Owner;

        protected override string GetClassNameCore() => "GaugeControl";

        protected override string GetNameCore()
        {
            string name = G.GetValue(AutomationProperties.NameProperty);
            return string.IsNullOrEmpty(name) ? "Gauge" : name;
        }

        public double Minimum => 0;
        public double Maximum => G.Maximum;
        public double Value { get => G.Value; set => G.Value = value; }
        public double SmallChange => 1;
        public double LargeChange => 10;
        public bool IsReadOnly => false;
        void IRangeValueProvider.SetValue(double value) => G.Value = value;
    }
}

public sealed class ValueChangedEventArgs : RoutedEventArgs
{
    public double OldValue { get; }
    public double NewValue { get; }

    public ValueChangedEventArgs(RoutedEvent routedEvent, double oldValue, double newValue)
        : base(routedEvent)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}
