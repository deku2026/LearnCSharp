using LearnMaui.Shared;

namespace LearnMaui.Maui_Stage03_UIThreadAsync;

public partial class MainPage : ContentPage
{
    private readonly AsyncWorkCoordinator _coordinator;

    public MainPage(AsyncWorkCoordinator coordinator)
    {
        InitializeComponent();
        _coordinator = coordinator;
        Content = new LearningStageView(StageCatalog.Create(coordinator));
    }

    protected override void OnDisappearing()
    {
        _coordinator.Cancel();
        base.OnDisappearing();
    }
}
