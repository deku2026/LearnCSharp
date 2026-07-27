using System.Globalization;
using Avalonia.Controls;
using Avalonia.Data.Converters;

namespace LearnAvalonia.Avalonia_Stage28_WpfMigration.Pages;

internal sealed class DemoFactoryConverter : IValueConverter
{
    public static readonly DemoFactoryConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is Func<Control> factory ? factory() : null;

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => null;
}
