// Doc:    第4部分-阶段9-数据绑定与MVVM.md §A5
// Title:  Converter 与 MultiBinding·诊断
// Stage:  Stage09
// Accept: 使用 Converter 与绑定诊断
// Notes:  能 partial 属性/源生成计算属性就少写 Converter；Converter 保持纯函数、无 IO。
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Data.Converters;

namespace LearnAvalonia.Avalonia_Stage09_DataBindingMVVM.Pages;

public partial class ConverterMultiBindingDemo : UserControl
{
    public ConverterMultiBindingDemo()
    {
        InitializeComponent();
    }
}

public sealed class BoolToOpacityConverter : IValueConverter
{
    public static readonly BoolToOpacityConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is true ? 1.0 : 0.4;

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
