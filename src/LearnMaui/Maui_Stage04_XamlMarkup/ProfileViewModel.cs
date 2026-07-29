using System.Collections.ObjectModel;
using System.ComponentModel;

namespace LearnMaui.Maui_Stage04_XamlMarkup;

public sealed class ProfileViewModel : INotifyPropertyChanged
{
    private string _displayName = "MAUI learner";

    public event PropertyChangedEventHandler? PropertyChanged;

    public string DisplayName
    {
        get => _displayName;
        set
        {
            if (_displayName == value)
            {
                return;
            }

            _displayName = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DisplayName)));
        }
    }

    public ObservableCollection<ProfileItem> Skills { get; } =
    [
        new("XAML", "Compiled"),
        new("C# Markup", "Strongly typed"),
        new("MVVM", "Thin view"),
    ];
}

public sealed record ProfileItem(string Name, string Status);

[ContentProperty(nameof(Text))]
public sealed class UpperExtension : IMarkupExtension<string>
{
    public string? Text { get; set; }

    public string ProvideValue(IServiceProvider serviceProvider)
    {
        return Text?.ToUpperInvariant() ?? string.Empty;
    }

    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
    {
        return ProvideValue(serviceProvider);
    }
}
