// Doc:    第3部分-阶段4-属性系统.md §3/§8
// Title:  FancyBadge —— 教学自定义控件（StyledProperty 演示）
// Stage:  Stage04
// Accept: 手写 StyledProperty 注册 + CLR 包装
// Notes:  展示 StyledProperty 的 validate/coerce/默认值与 class handler；用 Border 组合渲染。
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace LearnAvalonia.Avalonia_Stage04_PropertySystem.Pages;

public class FancyBadge : ContentControl
{
    public static readonly StyledProperty<string> TextProperty =
        AvaloniaProperty.Register<FancyBadge, string>(
            name: nameof(Text),
            defaultValue: string.Empty,
            inherits: false,
            defaultBindingMode: Avalonia.Data.BindingMode.OneWay,
            validate: static v => v is not null,
            coerce: static (_, v) => v?.Trim() ?? string.Empty,
            enableDataValidation: false);

    public static readonly StyledProperty<IBrush?> BadgeBackgroundProperty =
        AvaloniaProperty.Register<FancyBadge, IBrush?>(
            nameof(BadgeBackground),
            defaultValue: Brushes.DodgerBlue);

    static FancyBadge()
    {
        TextProperty.Changed.AddClassHandler<FancyBadge>((badge, _) =>
            badge.UpdateContent());
    }

    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public IBrush? BadgeBackground
    {
        get => GetValue(BadgeBackgroundProperty);
        set => SetValue(BadgeBackgroundProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == TextProperty || change.Property == BadgeBackgroundProperty)
        {
            UpdateContent();
        }
    }

    private void UpdateContent()
    {
        Content = new Border
        {
            Background = BadgeBackground ?? Brushes.DodgerBlue,
            CornerRadius = new CornerRadius(6),
            Padding = new Thickness(8, 4),
            Child = new TextBlock
            {
                Text = Text,
                Foreground = Brushes.White,
                FontSize = 13,
                VerticalAlignment = VerticalAlignment.Center,
            },
        };
    }
}
