using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace LearnMaui.Maui_Stage25_BindableBindingXaml;

public enum SignalSeverity
{
    Normal,
    Attention,
    Critical,
}

public sealed class BindingLabViewModel : INotifyPropertyChanged
{
    private int _count;
    private string _summary = "SourceGen compiled binding is active";
    private SignalSeverity _severity;

    public BindingLabViewModel()
    {
        AdvanceCommand = new Command(Advance);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Summary
    {
        get => _summary;
        private set => SetProperty(ref _summary, value);
    }

    public SignalSeverity Severity
    {
        get => _severity;
        private set => SetProperty(ref _severity, value);
    }

    public ICommand AdvanceCommand { get; }

    public void Advance()
    {
        _count++;
        Severity = (SignalSeverity)(_count % 3);
        Summary = $"update={_count}; severity={Severity}";
    }

    private void SetProperty<T>(
        ref T storage,
        T value,
        [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(storage, value))
        {
            return;
        }

        storage = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public sealed class SignalBadge : ContentView
{
    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text),
        typeof(string),
        typeof(SignalBadge),
        string.Empty,
        BindingMode.OneWay,
        validateValue: static (_, value) => value is string text && text.Length <= 160,
        propertyChanged: static (bindable, _, newValue) =>
            ((SignalBadge)bindable).UpdateText((string)newValue));

    public static readonly BindableProperty SeverityProperty = BindableProperty.Create(
        nameof(Severity),
        typeof(SignalSeverity),
        typeof(SignalBadge),
        SignalSeverity.Normal,
        BindingMode.OneWay,
        propertyChanged: static (bindable, _, newValue) =>
            ((SignalBadge)bindable).UpdateSeverity((SignalSeverity)newValue));

    private readonly Border _border;
    private readonly Label _label;

    public SignalBadge()
    {
        _label = new Label
        {
            FontAttributes = FontAttributes.Bold,
        };
        _border = new Border
        {
            Padding = new Thickness(12, 8),
            StrokeThickness = 2,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
            {
                CornerRadius = 10,
            },
            Content = _label,
        };
        Content = _border;
        UpdateText(Text);
        UpdateSeverity(Severity);
    }

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public SignalSeverity Severity
    {
        get => (SignalSeverity)GetValue(SeverityProperty);
        set => SetValue(SeverityProperty, value);
    }

    private void UpdateText(string value)
    {
        _label.Text = value;
    }

    private void UpdateSeverity(SignalSeverity value)
    {
        Color color = value switch
        {
            SignalSeverity.Normal => Colors.SeaGreen,
            SignalSeverity.Attention => Colors.DarkOrange,
            SignalSeverity.Critical => Colors.Crimson,
            _ => Colors.Gray,
        };
        _border.Stroke = color;
        _label.TextColor = color;
    }
}

public sealed record BindingTraceStep(
    int Order,
    string Layer,
    string Operation);

public static class BindingTrace
{
    public static IReadOnlyList<BindingTraceStep> PropertyToPixels() =>
    [
        new(1, "ViewModel", "PropertyChanged(propertyName)"),
        new(2, "Binding", "compiled getter reads the source"),
        new(3, "BindableObject", "SetValueCore stores effective value"),
        new(4, "BindableProperty", "propertyChanged callback runs"),
        new(5, "Handler mapper", "UpdateValue maps virtual view"),
        new(6, "Platform view", "native property invalidates measure/draw"),
    ];
}
