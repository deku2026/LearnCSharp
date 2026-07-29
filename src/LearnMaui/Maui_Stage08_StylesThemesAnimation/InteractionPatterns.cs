namespace LearnMaui.Maui_Stage08_StylesThemesAnimation;

public sealed class SelectAllBehavior : Behavior<Entry>
{
    protected override void OnAttachedTo(Entry bindable)
    {
        base.OnAttachedTo(bindable);
        bindable.Focused += OnFocused;
    }

    protected override void OnDetachingFrom(Entry bindable)
    {
        bindable.Focused -= OnFocused;
        base.OnDetachingFrom(bindable);
    }

    private static void OnFocused(object? sender, FocusEventArgs e)
    {
        if (sender is Entry entry)
        {
            entry.CursorPosition = 0;
            entry.SelectionLength = entry.Text?.Length ?? 0;
        }
    }
}

public abstract record CardModel(string Title);

public sealed record ArticleCard(string Title, string Summary) : CardModel(Title);

public sealed record MetricCard(string Title, double Value) : CardModel(Title);

public sealed class CardTemplateSelector : DataTemplateSelector
{
    public DataTemplate ArticleTemplate { get; set; } = new(() => new Label());

    public DataTemplate MetricTemplate { get; set; } = new(() => new ProgressBar());

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        return item is MetricCard ? MetricTemplate : ArticleTemplate;
    }
}

public sealed class AnimationCoordinator : IDisposable
{
    private CancellationTokenSource? _animationCancellation;

    public async Task PlayEnterAsync(View view, CancellationToken cancellationToken)
    {
        Cancel(view);
        _animationCancellation =
            CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        view.Opacity = 0;
        view.Scale = 0.92;
        try
        {
            await Task.WhenAll(
                view.FadeToAsync(1, 180, Easing.CubicOut),
                view.ScaleToAsync(1, 180, Easing.SpringOut))
                .WaitAsync(_animationCancellation.Token);
        }
        catch (OperationCanceledException)
        {
            view.CancelAnimations();
            throw;
        }
    }

    public void Cancel(View? view = null)
    {
        _animationCancellation?.Cancel();
        _animationCancellation?.Dispose();
        _animationCancellation = null;
        view?.CancelAnimations();
    }

    public void Dispose()
    {
        Cancel();
    }
}

public static class ThemeController
{
    public static AppTheme Toggle()
    {
        Application application = Application.Current
            ?? throw new InvalidOperationException("Application is not ready.");
        application.UserAppTheme = application.UserAppTheme == AppTheme.Dark
            ? AppTheme.Light
            : AppTheme.Dark;
        return application.UserAppTheme;
    }

    public static void SetBrandColor(Color color)
    {
        Application.Current!.Resources["BrandPrimary"] = color;
    }
}
