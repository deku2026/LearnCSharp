using LearnMaui.Shared;

namespace LearnMaui.Maui_Stage27_WindowShellDispatcher;

public partial class MainPage : ContentPage
{
    private readonly PageLifetime _lifetime = new();

    public MainPage(
        WindowJournal windowJournal,
        NavigationJournal navigationJournal,
        DeepLinkCoordinator deepLinks,
        IUiDispatcher dispatcher,
        SessionGate sessionGate)
    {
        InitializeComponent();
        StageHost.Content = new LearningStageView(StageCatalog.Create(
            windowJournal,
            navigationJournal,
            deepLinks,
            dispatcher,
            sessionGate,
            _lifetime));
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _lifetime.Appear();
    }

    protected override void OnDisappearing()
    {
        _lifetime.Disappear();
        base.OnDisappearing();
    }
}
