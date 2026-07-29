namespace LearnMaui.Maui_Stage07_NavigationShell;

public sealed class AppShell : Shell
{
    private readonly NavigationJournal _journal;

    public AppShell(MainPage mainPage, NavigationJournal journal)
    {
        _journal = journal;
        FlyoutBehavior = FlyoutBehavior.Disabled;
        Items.Add(new TabBar
        {
            Items =
            {
                new ShellContent
                {
                    Title = "Learn",
                    Route = RouteCatalog.Home,
                    Content = mainPage,
                },
            },
        });
        Navigating += OnNavigating;
        Navigated += OnNavigated;
    }

    private void OnNavigating(object? sender, ShellNavigatingEventArgs e)
    {
        _journal.Record($"Navigating {e.Current?.Location} → {e.Target.Location}");
    }

    private void OnNavigated(object? sender, ShellNavigatedEventArgs e)
    {
        _journal.Record($"Navigated source={e.Source}, current={e.Current.Location}");
    }
}

public sealed class ArticleDetailPage : ContentPage, IQueryAttributable
{
    public const string ArticleIdKey = "articleId";

    private readonly Label _articleLabel = new()
    {
        Text = "Waiting for route parameters",
        HorizontalOptions = LayoutOptions.Center,
        VerticalOptions = LayoutOptions.Center,
    };

    public ArticleDetailPage()
    {
        Title = "Article";
        Content = _articleLabel;
        Shell.SetPresentationMode(this, PresentationMode.Animated);
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        string? raw = query.TryGetValue(ArticleIdKey, out object? value)
            ? value?.ToString()
            : null;
        _articleLabel.Text = ArticleRoute.TryCreate(raw, out ArticleRoute route)
            ? $"Article: {route.Id}"
            : "Invalid article route";
    }
}

public sealed class LoginPage : ContentPage
{
    public LoginPage()
    {
        Title = "Login";
        Content = new Label
        {
            Text = "Authentication gate destination",
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
        };
        Shell.SetPresentationMode(this, PresentationMode.ModalAnimated);
    }
}
